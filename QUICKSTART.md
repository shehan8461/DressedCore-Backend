# Quick Start Guide - Dressed™ Platform

## Prerequisites

Before you begin, ensure you have the following installed:

- **Docker Desktop**: [Download here](https://www.docker.com/products/docker-desktop)
- **Docker Compose**: Usually included with Docker Desktop
- **Git**: For cloning the repository

## Quick Start (Docker)

### 1. Navigate to Backend Directory

```bash
cd "/Users/shehansalitha/Documents/CreditSource/DressedCore Backend"
```

### 2. Build and Start All Services

```bash
docker-compose up --build
```

This will:
- Build all microservice Docker images
- Start MySQL database with sample data
- Start phpMyAdmin for database management
- Start Auth, Design, and Quote services
- Start API Gateway
- Start React Frontend

### 3. Wait for Services to Start

The first build may take 5-10 minutes. Watch for these messages:
```
auth-service     | Now listening on: http://[::]:80
design-service   | Now listening on: http://[::]:80
quote-service    | Now listening on: http://[::]:80
api-gateway      | Now listening on: http://[::]:80
```

### 4. Access the Application

Once all services are running:

| Service | URL | Purpose |
|---------|-----|---------|
| **Frontend** | http://localhost:3000 | Main application UI |
| **API Gateway** | http://localhost:5000 | API entry point |
| **phpMyAdmin** | http://localhost:8080 | Database management |
| **Auth Service** | http://localhost:5001 | Direct auth access |
| **Design Service** | http://localhost:5002 | Direct design access |
| **Quote Service** | http://localhost:5003 | Direct quote access |

### 5. Login with Sample Accounts

**Designer Account**:
- Email: `designer@example.com`
- Password: `designer123`

**Supplier Account**:
- Email: `supplier@example.com`
- Password: `supplier123`

### 6. Access Database via phpMyAdmin

1. Go to http://localhost:8080
2. Login with:
   - **Server**: `mysql`
   - **Username**: `root`
   - **Password**: `rootpassword`

## Testing the APIs

### Using curl

**1. Register a new designer:**
```bash
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "newdesigner@example.com",
    "password": "password123",
    "firstName": "John",
    "lastName": "Doe",
    "userType": "Designer",
    "companyName": "Fashion Co",
    "contactNumber": "+1234567890",
    "address": "123 Fashion St"
  }'
```

**2. Login:**
```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "designer@example.com",
    "password": "designer123"
  }'
```

Save the token from the response.

**3. Create a design:**
```bash
curl -X POST http://localhost:5000/api/designs \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN_HERE" \
  -d '{
    "title": "Summer Dress Collection",
    "description": "Beautiful floral summer dress",
    "category": "Women",
    "fileUrls": ["https://example.com/design.pdf"],
    "quantity": 100,
    "specifications": "Cotton fabric, sizes S-XL"
  }'
```

**4. View all designs:**
```bash
curl http://localhost:5000/api/designs
```

**5. Submit a quote (as supplier):**
```bash
curl -X POST http://localhost:5000/api/quotes \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer SUPPLIER_TOKEN_HERE" \
  -d '{
    "designId": 1,
    "price": 1500.00,
    "deliveryTimeInDays": 30,
    "quoteText": "We can manufacture this design with premium quality",
    "termsAndConditions": "50% advance, 50% on delivery"
  }'
```

**6. View quotes for a design:**
```bash
curl http://localhost:5000/api/quotes/design/1 \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

## Stopping the Application

```bash
# Stop all containers
docker-compose down

# Stop and remove volumes (WARNING: This deletes all data)
docker-compose down -v
```

## Troubleshooting

### MySQL Connection Issues

If you see "Connection refused" errors:

```bash
# Check MySQL is running
docker-compose ps

# View MySQL logs
docker-compose logs mysql

# Restart just MySQL
docker-compose restart mysql
```

### Port Already in Use

If ports 3000, 5000, 5001, 5002, 5003, or 8080 are in use:

1. Find the process using the port:
```bash
# macOS/Linux
lsof -ti:5000

# Windows
netstat -ano | findstr :5000
```

2. Stop the process or change the port in `docker-compose.yml`

### Rebuilding After Changes

```bash
# Rebuild specific service
docker-compose build auth-service

# Rebuild all services
docker-compose build

# Rebuild and restart
docker-compose up --build
```

### View Service Logs

```bash
# All services
docker-compose logs -f

# Specific service
docker-compose logs -f auth-service
docker-compose logs -f design-service
docker-compose logs -f quote-service
```

### Database Not Initializing

If sample data isn't loading:

```bash
# Stop everything
docker-compose down -v

# Rebuild and restart
docker-compose up --build

# Or manually import SQL
docker exec -i dressed-mysql mysql -uroot -prootpassword < database/init.sql
```

## Development Mode

### Running Services Locally (Without Docker)

1. **Install .NET 6 SDK**
   Download from: https://dotnet.microsoft.com/download/dotnet/6.0

2. **Start MySQL Locally**
   - Use XAMPP or install MySQL separately
   - Import `database/init.sql`

3. **Update Connection Strings**
   
   Edit `appsettings.json` in each service:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=dressed_auth;User=root;Password=yourpassword;"
     }
   }
   ```

4. **Run Services**
   
   Open separate terminals for each:
   
   ```bash
   # Terminal 1 - Auth Service
   cd "src/Services/Auth.Service"
   dotnet run
   
   # Terminal 2 - Design Service
   cd "src/Services/Design.Service"
   dotnet run
   
   # Terminal 3 - Quote Service
   cd "src/Services/Quote.Service"
   dotnet run
   
   # Terminal 4 - API Gateway
   cd "src/Gateway/ApiGateway"
   dotnet run
   ```

5. **Run Frontend**
   
   ```bash
   cd "../DressedCore Frontend"
   npm install
   npm run dev
   ```

## API Documentation

### Swagger UI

Each service has Swagger documentation available in development mode:

- Auth Service: http://localhost:5001/swagger
- Design Service: http://localhost:5002/swagger
- Quote Service: http://localhost:5003/swagger

### Postman Collection

Import the API endpoints into Postman for easier testing:

1. Create new collection: "Dressed API"
2. Add requests for each endpoint
3. Use environment variables for token management

### Sample Postman Environment

```json
{
  "name": "Dressed Local",
  "values": [
    {
      "key": "base_url",
      "value": "http://localhost:5000",
      "enabled": true
    },
    {
      "key": "auth_token",
      "value": "",
      "enabled": true
    }
  ]
}
```

## Database Management

### Viewing Data

Use phpMyAdmin (http://localhost:8080) or connect via any MySQL client:

```
Host: localhost
Port: 3306
Username: root
Password: rootpassword
```

### Sample Queries

```sql
-- View all users
SELECT * FROM dressed_auth.Users;

-- View all designs
SELECT * FROM dressed_design.Designs;

-- View all quotes
SELECT * FROM dressed_quote.Quotes;

-- Designer with most designs
SELECT d.CompanyName, COUNT(*) as DesignCount
FROM dressed_design.Designs des
JOIN dressed_design.Designers d ON des.DesignerId = d.Id
GROUP BY d.CompanyName
ORDER BY DesignCount DESC;
```

## Next Steps

1. **Implement Frontend**:
   - Update React components in `DressedCore Frontend/src`
   - Create designer and supplier dashboards
   - Implement design submission form
   - Add quote viewing and submission

2. **Add More Services**:
   - Order Service for order management
   - Communication Service for messaging
   - Notification Service for alerts

3. **Deploy to Cloud**:
   - Push images to Azure Container Registry
   - Deploy to Azure Kubernetes Service
   - Configure Azure Database for MySQL
   - Set up Application Insights

## Support

For issues:
1. Check logs: `docker-compose logs`
2. Verify all services are healthy: `docker-compose ps`
3. Check database connectivity: Try phpMyAdmin
4. Review README.md and ARCHITECTURE.md

## Useful Commands

```bash
# Start in background
docker-compose up -d

# View running containers
docker ps

# Stop specific service
docker-compose stop auth-service

# Restart specific service
docker-compose restart auth-service

# Remove all containers
docker-compose rm -f

# Clean up everything
docker-compose down -v --remove-orphans

# View container resource usage
docker stats
```

---

**Happy Coding! 🚀**
