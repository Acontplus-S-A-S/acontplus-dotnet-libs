# Acontplus.Services Advanced Usage Guide

This guide demonstrates how to use the `AdvancedUsageController` in the Acontplus.TestApi application to test and understand all the advanced usage patterns from the Acontplus.Services README.

## 🚀 Quick Start

### 1. Run the Application

```bash
cd apps/src/Acontplus.TestApi
dotnet run
```

### 2. Test Basic Endpoints

Start with the basic endpoints to verify everything is working:

```bash
# Test basic hello endpoint
curl http://localhost:5000/api/AdvancedUsage/basic/hello

# Test basic product endpoint
curl http://localhost:5000/api/AdvancedUsage/basic/products/123
```

## 📚 Usage Examples by Category

### 🟢 Basic Usage Examples

#### `/api/AdvancedUsage/basic/hello`
- **Purpose**: Demonstrates simple caching with request context
- **Features**: Basic caching, correlation ID generation
- **Use Case**: Getting started with Acontplus.Services

#### `/api/AdvancedUsage/basic/products/{id}`
- **Purpose**: Shows product retrieval with caching
- **Features**: Cache-aside pattern, request correlation
- **Use Case**: Simple data caching scenarios

### 🟡 Intermediate Usage Examples

#### `/api/AdvancedUsage/intermediate/content`
- **Purpose**: Device-aware content with circuit breaker protection
- **Features**: Device detection, caching, circuit breaker
- **Use Case**: Content optimization for different devices

#### `/api/AdvancedUsage/intermediate/health`
- **Purpose**: Health monitoring for services
- **Features**: Circuit breaker status, cache statistics
- **Use Case**: Service health monitoring

#### `/api/AdvancedUsage/intermediate/cache-stats`
- **Purpose**: Cache performance monitoring
- **Features**: Detailed cache statistics, performance metrics
- **Use Case**: Cache optimization and monitoring

### 🔴 Enterprise Usage Examples

#### `/api/AdvancedUsage/enterprise/dashboard`
- **Purpose**: Multi-tenant dashboard with comprehensive monitoring
- **Features**: Tenant isolation, device awareness, circuit breaker
- **Use Case**: Enterprise multi-tenant applications

#### `/api/AdvancedUsage/enterprise/audit`
- **Purpose**: Audit logging with comprehensive context
- **Features**: Full request context, audit trail, security
- **Use Case**: Compliance and security auditing

#### `/api/AdvancedUsage/enterprise/security-status`
- **Purpose**: Security status and monitoring
- **Features**: Security headers, CSP nonce, circuit breaker status
- **Use Case**: Security monitoring and compliance

#### `/api/AdvancedUsage/enterprise/tenant-data/{dataType}`
- **Purpose**: Multi-tenant data with isolation
- **Features**: Tenant isolation, caching, metadata
- **Use Case**: Multi-tenant data management

### 🔧 Advanced Service Patterns

#### `/api/AdvancedUsage/advanced/hybrid-cache/{key}`
- **Purpose**: Hybrid caching with fallback
- **Features**: Cache fallback, error handling
- **Use Case**: Resilient caching strategies

#### `/api/AdvancedUsage/advanced/resilient/{operation}`
- **Purpose**: Composite resilience with fallback
- **Features**: Retry policies, fallback values, circuit breaker
- **Use Case**: High-availability service patterns

#### `/api/AdvancedUsage/advanced/adaptive-content/{contentId}`
- **Purpose**: Device-aware content optimization
- **Features**: Device detection, content optimization, caching
- **Use Case**: Responsive content delivery

## 🧪 Testing Scenarios

### Device Testing

Test how the API responds to different device types:

```bash
# Mobile device
curl -H "User-Agent: Mozilla/5.0 (iPhone; CPU iPhone OS 14_0 like Mac OS X) AppleWebKit/605.1.15" \
  http://localhost:5000/api/AdvancedUsage/intermediate/content

# Tablet device
curl -H "User-Agent: Mozilla/5.0 (iPad; CPU OS 14_0 like Mac OS X) AppleWebKit/605.1.15" \
  http://localhost:5000/api/AdvancedUsage/intermediate/content

# Desktop device
curl -H "User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36" \
  http://localhost:5000/api/AdvancedUsage/intermediate/content
```

### Cache Testing

Test caching behavior and performance:

```bash
# First request (cache miss)
curl http://localhost:5000/api/AdvancedUsage/basic/products/123

# Second request (cache hit - should be faster)
curl http://localhost:5000/api/AdvancedUsage/basic/products/123

# Check cache statistics
curl http://localhost:5000/api/AdvancedUsage/intermediate/cache-stats
```

### Circuit Breaker Testing

Test resilience patterns:

```bash
# Check circuit breaker status
curl http://localhost:5000/api/AdvancedUsage/intermediate/health

# Test resilient operations
curl http://localhost:5000/api/AdvancedUsage/advanced/resilient/database-query
curl http://localhost:5000/api/AdvancedUsage/advanced/resilient/external-api
curl http://localhost:5000/api/AdvancedUsage/advanced/resilient/payment-gateway
```

### Multi-Tenant Testing

Test tenant isolation and data separation:

```bash
# Test with different tenants
curl -H "X-Tenant-ID: tenant-a" -H "X-Client-ID: web-app" \
  http://localhost:5000/api/AdvancedUsage/enterprise/tenant-data/config

curl -H "X-Tenant-ID: tenant-b" -H "X-Client-ID: mobile-app" \
  http://localhost:5000/api/AdvancedUsage/enterprise/tenant-data/config

curl -H "X-Tenant-ID: premium-tenant-001" -H "X-Client-ID: admin-portal" \
  http://localhost:5000/api/AdvancedUsage/enterprise/tenant-data/config
```

## 🔧 Configuration

### Required Headers

For enterprise endpoints, you may need to set these headers:

- `X-Client-ID`: Identifies the client application
- `X-Tenant-ID`: Identifies the tenant for multi-tenant scenarios
- `Authorization`: Bearer token for authenticated endpoints

### Environment Variables

Set these environment variables for testing:

```bash
export ASPNETCORE_ENVIRONMENT=Development
export ASPNETCORE_URLS=http://localhost:5000
```

### Configuration Files

The application uses these configuration files:

- `appsettings.json`: Base configuration
- `appsettings.Development.json`: Development-specific settings

## 📊 Monitoring and Health Checks

### Health Endpoints

```bash
# Overall health
curl http://localhost:5000/health

# Circuit breaker health
curl http://localhost:5000/health/circuit-breaker

# Cache health
curl http://localhost:5000/health/cache
```

### Metrics to Monitor

1. **Cache Performance**
   - Hit rate percentage
   - Memory usage
   - Eviction count

2. **Circuit Breaker Status**
   - Open/Closed state
   - Failure count
   - Success rate

3. **Response Times**
   - Cache hit vs. miss performance
   - Circuit breaker overhead
   - Device detection performance

## 🚨 Troubleshooting

### Common Issues

#### Cache Not Working
- Check if Redis is running (if using distributed cache)
- Verify cache configuration in `appsettings.json`
- Check cache health endpoint

#### Circuit Breaker Always Closed
- Verify resilience configuration
- Check if circuit breaker is enabled
- Monitor health endpoints

#### Device Detection Issues
- Verify User-Agent headers
- Check device detection patterns
- Test with known device strings

#### Authorization Failures
- Verify client ID and tenant ID headers
- Check authorization policies
- Ensure proper authentication tokens

### Debug Information

Enable debug logging by setting:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Acontplus.Services": "Debug"
    }
  }
}
```

## 🔍 Performance Testing

### Load Testing

Test the API under load to see how caching and circuit breakers perform:

```bash
# Install Apache Bench
sudo apt-get install apache2-utils

# Test basic endpoint
ab -n 1000 -c 10 http://localhost:5000/api/AdvancedUsage/basic/hello

# Test intermediate endpoint
ab -n 1000 -c 10 http://localhost:5000/api/AdvancedUsage/intermediate/content
```

### Cache Performance

Monitor cache performance over time:

```bash
# Check cache stats before load test
curl http://localhost:5000/api/AdvancedUsage/intermediate/cache-stats

# Run load test
ab -n 1000 -c 10 http://localhost:5000/api/AdvancedUsage/basic/products/123

# Check cache stats after load test
curl http://localhost:5000/api/AdvancedUsage/intermediate/cache-stats
```

## 📈 Best Practices

### Caching
- Use descriptive cache keys
- Set appropriate TTL values
- Monitor cache hit rates
- Implement cache invalidation strategies

### Circuit Breakers
- Use different circuit breakers for different services
- Monitor circuit breaker states
- Implement fallback strategies
- Test failure scenarios

### Device Detection
- Don't rely solely on User-Agent strings
- Implement progressive enhancement
- Test with various devices
- Cache device-specific content

### Multi-Tenancy
- Always validate tenant IDs
- Implement proper isolation
- Use tenant-specific cache keys
- Monitor tenant-specific metrics

## 🔗 Related Documentation

- [Acontplus.Services README](../src/Acontplus.Services/README.md)
- [Enterprise Examples Controller](./Controllers/EnterpriseExamplesController.cs)
- [HTTP Test File](./AdvancedUsage.http)

## 🤝 Contributing

When adding new examples:

1. Follow the existing pattern structure
2. Include comprehensive documentation
3. Add corresponding HTTP test cases
4. Update this guide
5. Test with different scenarios

## 📞 Support

For issues or questions:

- Check the troubleshooting section
- Review the health endpoints
- Check application logs
- Create an issue in the repository
