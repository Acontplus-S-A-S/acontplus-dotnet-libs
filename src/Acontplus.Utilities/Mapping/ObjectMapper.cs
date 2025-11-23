namespace Acontplus.Utilities.Mapping;

using System.Reflection;

/// <summary>
/// A lightweight object mapper that provides AutoMapper-like functionality.
/// </summary>
public static class ObjectMapper
{
    private static readonly Dictionary<string, MappingConfiguration> MappingConfigurations = new Dictionary<string, MappingConfiguration>();

    /// <summary>
    /// Creates a mapping configuration between source and target types.
    /// </summary>
    public static MappingConfiguration<TSource, TTarget> CreateMap<TSource, TTarget>()
    {
        var config = new MappingConfiguration<TSource, TTarget>();
        var key = GetMappingKey(typeof(TSource), typeof(TTarget));
        MappingConfigurations[key] = config;
        return config;
    }

    /// <summary>
    /// Maps a source object to a new instance of a target type.
    /// </summary>
    public static TTarget? Map<TSource, TTarget>(TSource source)
    {
        if (source == null)
            return default;

        var targetType = typeof(TTarget);

        // Create a new instance, handling required constructor parameters if needed
        var target = CreateInstance<TSource, TTarget>(source);

        return Map(source, target);
    }

    /// <summary>
    /// Maps a source object to an existing target object.
    /// </summary>
    public static TTarget Map<TSource, TTarget>(TSource source, TTarget target)
    {
        ArgumentNullException.ThrowIfNull(target);
        
        if (source == null)
            return target;

        // Check if we have a configuration for this mapping
        var key = GetMappingKey(typeof(TSource), typeof(TTarget));
        if (MappingConfigurations.TryGetValue(key, out var configuration))
        {
            return (TTarget)configuration.Map(source, target);
        }

        // If no configuration exists, perform standard property mapping
        return MapProperties(source, target);
    }

    /// <summary>
    /// Creates an instance of the target type, using constructor parameters matched from the source if necessary.
    /// </summary>
    private static TTarget CreateInstance<TSource, TTarget>(TSource source)
    {
        var targetType = typeof(TTarget);
        var sourceType = typeof(TSource);

        // If the type has a parameterless constructor, use it
        var parameterlessCtor = targetType.GetConstructor(Type.EmptyTypes);
        if (parameterlessCtor != null)
        {
            var instance = Activator.CreateInstance(targetType);
            if (instance == null)
                throw new InvalidOperationException($"Failed to create instance of {targetType.Name}");
            return (TTarget)instance;
        }

        // Find the constructor with the most parameters that we can satisfy
        var ctors = targetType.GetConstructors()
            .OrderByDescending(c => c.GetParameters().Length);

        foreach (var ctor in ctors)
        {
            var parameters = ctor.GetParameters();
            var paramValues = new object?[parameters.Length];
            var canSatisfyAllParams = true;

            for (var i = 0; i < parameters.Length; i++)
            {
                var param = parameters[i];
                var paramName = param.Name;
                if (paramName == null)
                {
                    canSatisfyAllParams = false;
                    break;
                }
                
                var sourceProp = sourceType.GetProperty(paramName,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

                if (sourceProp != null)
                {
                    var sourceValue = sourceProp.GetValue(source);

                    // Try to convert the value if needed
                    if (sourceValue != null && !param.ParameterType.IsAssignableFrom(sourceProp.PropertyType))
                    {
                        try
                        {
                            sourceValue = Convert.ChangeType(sourceValue, param.ParameterType);
                        }
                        catch
                        {
                            // If conversion fails, check if we need to map a complex type
                            if (!IsSimpleType(sourceProp.PropertyType) && !IsSimpleType(param.ParameterType))
                            {
                                // Try to create and map the complex parameter
                                var mappingMethod = typeof(ObjectMapper).GetMethod("Map",
                                    [sourceProp.PropertyType]);

                                if (mappingMethod != null)
                                {
                                    try
                                    {
                                        var genericMethod = mappingMethod.MakeGenericMethod(sourceProp.PropertyType, param.ParameterType);
                                        sourceValue = genericMethod.Invoke(null, [sourceValue]);
                                    }
                                    catch
                                    {
                                        // If mapping fails, we can't satisfy this parameter
                                        canSatisfyAllParams = false;
                                        break;
                                    }
                                }
                                else
                                {
                                    canSatisfyAllParams = false;
                                    break;
                                }
                            }
                            else
                            {
                                canSatisfyAllParams = false;
                                break;
                            }
                        }
                    }

                    paramValues[i] = sourceValue;
                }
                else if (param.HasDefaultValue)
                {
                    // Use default value for the parameter if available
                    paramValues[i] = param.DefaultValue;
                }
                else
                {
                    // Can't satisfy this parameter
                    canSatisfyAllParams = false;
                    break;
                }
            }

            if (canSatisfyAllParams)
            {
                try
                {
                    var instance = ctor.Invoke(paramValues);
                    if (instance != null)
                        return (TTarget)instance;
                }
                catch
                {
                    // If constructor invocation fails, try the next constructor
                    continue;
                }
            }
        }

        // If we can't find a suitable constructor, throw an exception
        throw new InvalidOperationException($"Cannot create an instance of {targetType.Name}. No suitable constructor found that can be satisfied with the source properties.");
    }

    /// <summary>
    /// Maps an object to another type without using a predefined mapping configuration.
    /// </summary>
    private static TTarget MapProperties<TSource, TTarget>(TSource source, TTarget target)
    {
        var sourceProperties = typeof(TSource).GetProperties();
        var targetProperties = typeof(TTarget).GetProperties();

        foreach (var sourceProp in sourceProperties)
        {
            var targetProp = targetProperties.FirstOrDefault(p =>
                string.Equals(p.Name, sourceProp.Name, StringComparison.OrdinalIgnoreCase) &&
                p.CanWrite);

            if (targetProp == null)
                continue;

            var sourceValue = sourceProp.GetValue(source);
            if (sourceValue == null)
                continue;

            // Handle direct assignment if types are compatible
            if (targetProp.PropertyType.IsAssignableFrom(sourceProp.PropertyType))
            {
                targetProp.SetValue(target, sourceValue);
                continue;
            }

            // Handle collections
            if (IsCollection(sourceProp.PropertyType) && IsCollection(targetProp.PropertyType))
            {
                MapCollection(sourceValue, target!, targetProp);
                continue;
            }

            // Handle complex types that need recursive mapping
            if (!IsSimpleType(sourceProp.PropertyType) && !IsSimpleType(targetProp.PropertyType))
            {
                var nestedTargetValue = targetProp.GetValue(target);
                if (nestedTargetValue == null)
                {
                    // Create an instance if null, handling constructor parameters if needed
                    try
                    {
                        var createInstanceMethod = typeof(ObjectMapper).GetMethod("CreateInstance",
                            BindingFlags.NonPublic | BindingFlags.Static);
                        if (createInstanceMethod != null)
                        {
                            var genericMethod = createInstanceMethod.MakeGenericMethod(sourceProp.PropertyType, targetProp.PropertyType);
                            nestedTargetValue = genericMethod.Invoke(null, [sourceValue]);
                        }
                    }
                    catch
                    {
                        // If CreateInstance fails, try Activator as fallback
                        nestedTargetValue = Activator.CreateInstance(targetProp.PropertyType);
                    }
                }

                if (nestedTargetValue != null)
                {
                    var mapMethod = typeof(ObjectMapper)
                        .GetMethod("Map", [sourceProp.PropertyType, targetProp.PropertyType]);
                    if (mapMethod != null)
                    {
                        var nestedMappedValue = mapMethod.Invoke(null, [sourceValue, nestedTargetValue]);
                        targetProp.SetValue(target, nestedMappedValue);
                    }
                }
                continue;
            }

            // Try type conversion for simple types
            try
            {
                var convertedValue = Convert.ChangeType(sourceValue, targetProp.PropertyType);
                targetProp.SetValue(target, convertedValue);
            }
            catch (InvalidCastException)
            {
                // Silently ignore failed type conversions
            }
        }

        return target;
    }

    private static void MapCollection(object sourceCollection, object target, PropertyInfo targetProperty)
    {
        var targetCollectionType = targetProperty.PropertyType;
        var targetElementType = GetElementType(targetCollectionType);

        if (targetElementType == null)
            return;

        // Handle IEnumerable/ICollection/IList type targets
        if (targetCollectionType.IsInterface)
        {
            var concreteType = typeof(List<>).MakeGenericType(targetElementType);
            var targetCollection = Activator.CreateInstance(concreteType);
            if (targetCollection == null)
                return;
                
            var addMethod = concreteType.GetMethod("Add");
            if (addMethod == null)
                return;

            foreach (var sourceItem in (IEnumerable)sourceCollection)
            {
                if (sourceItem == null)
                    continue;

                var sourceItemType = sourceItem.GetType();

                // Map simple types directly
                if (IsSimpleType(sourceItemType) && targetElementType.IsAssignableFrom(sourceItemType))
                {
                    addMethod.Invoke(targetCollection, [sourceItem]);
                }
                // Map complex types
                else if (!IsSimpleType(sourceItemType))
                {
                    // Create a new instance of target element type, handling constructor params if needed
                    object? targetItem;
                    try
                    {
                        var createInstanceMethod = typeof(ObjectMapper).GetMethod("CreateInstance",
                            BindingFlags.NonPublic | BindingFlags.Static);
                        if (createInstanceMethod == null)
                            continue;
                            
                        var genericMethod = createInstanceMethod.MakeGenericMethod(sourceItemType, targetElementType);
                        targetItem = genericMethod.Invoke(null, [sourceItem]);
                    }
                    catch
                    {
                        // Fallback to Activator
                        targetItem = Activator.CreateInstance(targetElementType);
                    }

                    if (targetItem == null)
                        continue;

                    var mapMethod = typeof(ObjectMapper)
                        .GetMethod("Map", [sourceItemType, targetElementType]);
                    if (mapMethod == null)
                        continue;
                        
                    var mappedItem = mapMethod.Invoke(null, [sourceItem, targetItem]);
                    if (mappedItem != null)
                        addMethod.Invoke(targetCollection, [mappedItem]);
                }
            }

            targetProperty.SetValue(target, targetCollection);
        }
        // Handle concrete collection types
        else if (!targetCollectionType.IsAbstract)
        {
            var targetCollection = Activator.CreateInstance(targetCollectionType);
            if (targetCollection == null)
                return;
                
            var addMethod = targetCollectionType.GetMethod("Add");

            if (addMethod != null)
            {
                foreach (var sourceItem in (IEnumerable)sourceCollection)
                {
                    if (sourceItem == null)
                        continue;

                    var sourceItemType = sourceItem.GetType();

                    // Map simple types directly
                    if (IsSimpleType(sourceItemType) && targetElementType.IsAssignableFrom(sourceItemType))
                    {
                        addMethod.Invoke(targetCollection, [sourceItem]);
                    }
                    // Map complex types
                    else if (!IsSimpleType(sourceItemType))
                    {
                        // Create a new instance of target element type, handling constructor params if needed
                        object? targetItem;
                        try
                        {
                            var createInstanceMethod = typeof(ObjectMapper).GetMethod("CreateInstance",
                                BindingFlags.NonPublic | BindingFlags.Static);
                            if (createInstanceMethod == null)
                                continue;
                                
                            var genericMethod = createInstanceMethod.MakeGenericMethod(sourceItemType, targetElementType);
                            targetItem = genericMethod.Invoke(null, [sourceItem]);
                        }
                        catch
                        {
                            // Fallback to Activator
                            targetItem = Activator.CreateInstance(targetElementType);
                        }

                        if (targetItem == null)
                            continue;

                        var mapMethod = typeof(ObjectMapper)
                            .GetMethod("Map", [sourceItemType, targetElementType]);
                        if (mapMethod == null)
                            continue;
                            
                        var mappedItem = mapMethod.Invoke(null, [sourceItem, targetItem]);
                        if (mappedItem != null)
                            addMethod.Invoke(targetCollection, [mappedItem]);
                    }
                }

                targetProperty.SetValue(target, targetCollection);
            }
        }
    }

    private static bool IsCollection(Type type)
    {
        return type != typeof(string) && typeof(IEnumerable).IsAssignableFrom(type);
    }

    private static Type? GetElementType(Type collectionType)
    {
        if (collectionType.IsArray)
            return collectionType.GetElementType();

        if (collectionType.IsGenericType &&
            (typeof(IEnumerable<>).IsAssignableFrom(collectionType.GetGenericTypeDefinition()) ||
             typeof(ICollection<>).IsAssignableFrom(collectionType.GetGenericTypeDefinition()) ||
             typeof(IList<>).IsAssignableFrom(collectionType.GetGenericTypeDefinition())))
        {
            return collectionType.GetGenericArguments()[0];
        }

        var interfaces = collectionType.GetInterfaces();
        foreach (var iface in interfaces)
        {
            if (iface.IsGenericType &&
                (iface.GetGenericTypeDefinition() == typeof(IEnumerable<>) ||
                 iface.GetGenericTypeDefinition() == typeof(ICollection<>) ||
                 iface.GetGenericTypeDefinition() == typeof(IList<>)))
            {
                return iface.GetGenericArguments()[0];
            }
        }

        return null;
    }

    private static bool IsSimpleType(Type? type)
    {
        if (type == null)
            return false;
            
        return type.IsPrimitive
            || type == typeof(string)
            || type == typeof(decimal)
            || type == typeof(DateTime)
            || type == typeof(DateTimeOffset)
            || type == typeof(TimeSpan)
            || type == typeof(Guid)
            || type.IsEnum
            || (Nullable.GetUnderlyingType(type) is Type underlyingType && IsSimpleType(underlyingType));
    }

    private static string GetMappingKey(Type sourceType, Type targetType)
    {
        return $"{sourceType.FullName}=>{targetType.FullName}";
    }

    /// <summary>
    /// Base mapping configuration class.
    /// </summary>
    public abstract class MappingConfiguration
    {
        public abstract object Map(object source, object target);
    }

    /// <summary>
    /// Mapping configuration for source and target types.
    /// </summary>
    public class MappingConfiguration<TSource, TTarget> : MappingConfiguration
    {
        private readonly List<Action<TSource, TTarget>> _mappingActions = new List<Action<TSource, TTarget>>();
        private readonly Dictionary<string, LambdaExpression> _customMappings = new Dictionary<string, LambdaExpression>();
        private bool _ignoreUnmappedProperties = false;
        private Dictionary<string, string> _constructorParameterMappings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Configures a custom mapping for a target property.
        /// </summary>
        public MappingConfiguration<TSource, TTarget> ForMember<TProperty>(
            Expression<Func<TTarget, TProperty>> destinationMember,
            Expression<Func<TSource, TProperty>> sourceMember)
        {
            var memberName = GetMemberName(destinationMember);
            if (memberName != null)
                _customMappings[memberName] = sourceMember;
            return this;
        }

        /// <summary>
        /// Configures a custom mapping function for a target property.
        /// </summary>
        public MappingConfiguration<TSource, TTarget> ForMember<TProperty>(
            Expression<Func<TTarget, TProperty>> destinationMember,
            Func<TSource, TProperty> mappingFunction)
        {
            _mappingActions.Add((source, target) =>
            {
                if (destinationMember.Body is MemberExpression memberExp &&
                    memberExp.Member is PropertyInfo property)
                {
                    var value = mappingFunction(source);
                    property.SetValue(target, value);
                }
            });
            return this;
        }

        /// <summary>
        /// Maps a source property to a constructor parameter.
        /// </summary>
        public MappingConfiguration<TSource, TTarget> ForCtorParam(string paramName, string sourcePropertyName)
        {
            _constructorParameterMappings[paramName] = sourcePropertyName;
            return this;
        }

        /// <summary>
        /// Maps a source property to a constructor parameter using expressions.
        /// </summary>
        public MappingConfiguration<TSource, TTarget> ForCtorParam<TProperty>(
            string paramName,
            Expression<Func<TSource, TProperty>> sourceMember)
        {
            var memberName = GetMemberNameFromSource(sourceMember);
            if (memberName != null)
                _constructorParameterMappings[paramName] = memberName;
            return this;
        }

        /// <summary>
        /// Ignores a specific property during mapping.
        /// </summary>
        public MappingConfiguration<TSource, TTarget> Ignore<TProperty>(
            Expression<Func<TTarget, TProperty>> destinationMember)
        {
            var memberName = GetMemberName(destinationMember);
            if (memberName != null)
                _customMappings[memberName] = null!; // null indicates ignore
            return this;
        }

        /// <summary>
        /// Ignores all unmapped properties.
        /// </summary>
        public MappingConfiguration<TSource, TTarget> IgnoreUnmappedProperties()
        {
            _ignoreUnmappedProperties = true;
            return this;
        }

        public override object Map(object source, object target)
        {
            var typedSource = (TSource)source;
            var typedTarget = (TTarget)target;

            // Apply any custom mapping functions
            foreach (var action in _mappingActions)
            {
                action(typedSource, typedTarget);
            }

            // Map properties
            var sourceProperties = typeof(TSource).GetProperties();
            var targetProperties = typeof(TTarget).GetProperties();

            foreach (var targetProp in targetProperties)
            {
                // Skip if property is configured to be ignored
                if (_customMappings.TryGetValue(targetProp.Name, out var customMapping) && customMapping == null)
                    continue;

                // Apply custom mapping if configured
                if (customMapping != null)
                {
                    var func = customMapping.Compile();
                    var value = func.DynamicInvoke(typedSource);
                    targetProp.SetValue(typedTarget, value);
                    continue;
                }

                // Otherwise look for matching property in source
                var sourceProp = sourceProperties.FirstOrDefault(p =>
                    string.Equals(p.Name, targetProp.Name, StringComparison.OrdinalIgnoreCase));

                if (sourceProp != null)
                {
                    var sourceValue = sourceProp.GetValue(typedSource);
                    if (sourceValue == null)
                        continue;

                    // Handle direct assignment if types are compatible
                    if (targetProp.PropertyType.IsAssignableFrom(sourceProp.PropertyType))
                    {
                        targetProp.SetValue(typedTarget, sourceValue);
                        continue;
                    }

                    // Handle collections
                    if (IsCollection(sourceProp.PropertyType) && IsCollection(targetProp.PropertyType))
                    {
                        MapCollection(sourceValue, typedTarget!, targetProp);
                        continue;
                    }

                    // Handle complex types that need recursive mapping
                    if (!IsSimpleType(sourceProp.PropertyType) && !IsSimpleType(targetProp.PropertyType))
                    {
                        var nestedTargetValue = targetProp.GetValue(typedTarget);
                        if (nestedTargetValue == null)
                        {
                            // Create an instance if null, handling constructor parameters if needed
                            try
                            {
                                var createInstanceMethod = typeof(ObjectMapper).GetMethod("CreateInstance",
                                    BindingFlags.NonPublic | BindingFlags.Static);
                                if (createInstanceMethod != null)
                                {
                                    var genericMethod = createInstanceMethod.MakeGenericMethod(sourceProp.PropertyType, targetProp.PropertyType);
                                    nestedTargetValue = genericMethod.Invoke(null, [sourceValue]);
                                }
                            }
                            catch
                            {
                                // If CreateInstance fails, try Activator as fallback
                                nestedTargetValue = Activator.CreateInstance(targetProp.PropertyType);
                            }
                        }

                        if (nestedTargetValue != null)
                        {
                            var mapMethod = typeof(ObjectMapper)
                                .GetMethod("Map", [sourceProp.PropertyType, targetProp.PropertyType]);
                            if (mapMethod != null)
                            {
                                var nestedMappedValue = mapMethod.Invoke(null, [sourceValue, nestedTargetValue]);
                                targetProp.SetValue(typedTarget, nestedMappedValue);
                            }
                        }
                        continue;
                    }

                    // Try type conversion for simple types
                    try
                    {
                        var convertedValue = Convert.ChangeType(sourceValue, targetProp.PropertyType);
                        targetProp.SetValue(typedTarget, convertedValue);
                    }
                    catch (InvalidCastException)
                    {
                        // Silently ignore failed type conversions
                    }
                }
                else if (!_ignoreUnmappedProperties)
                {
                    // Missing source property, use default value if not ignoring unmapped properties
                    targetProp.SetValue(typedTarget, GetDefaultValue(targetProp.PropertyType));
                }
            }

            return typedTarget;
        }

        private string? GetMemberName<TProperty>(Expression<Func<TTarget, TProperty>> expression)
        {
            if (expression.Body is not MemberExpression memberExp)
                throw new ArgumentException("Expression must be a member expression");
                
            return memberExp.Member.Name;
        }

        private string? GetMemberNameFromSource<TProperty>(Expression<Func<TSource, TProperty>> expression)
        {
            if (expression.Body is not MemberExpression memberExp)
                throw new ArgumentException("Expression must be a member expression");
                
            return memberExp.Member.Name;
        }

        private static object? GetDefaultValue(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        // For use by the CreateInstance method
        internal Dictionary<string, string> GetConstructorParameterMappings()
        {
            return _constructorParameterMappings;
        }
    }
}
