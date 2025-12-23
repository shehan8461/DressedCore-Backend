# Dressed™ - Fashion Design & Manufacturing Platform

## System Architecture

### Overview
Dressed™ is a microservices-based platform that connects fashion designers with garment manufacturers, facilitating design submissions, quote management, order placement, and payment processing.

### Microservices Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                         API Gateway (Ocelot)                     │
│                         Port: 5000                               │
└────────┬──────────────────┬─────────────────┬────────────────────┘
         │                  │                 │
         ▼                  ▼                 ▼
┌──────────────┐  ┌──────────────┐  ┌──────────────┐
│ Auth Service │  │Design Service│  │Quote Service │
│   Port 5001  │  │   Port 5002  │  │   Port 5003  │
└──────┬───────┘  └──────┬───────┘  └──────┬───────┘
       │                 │                  │
       └─────────────────┼──────────────────┘
                         ▼
                  ┌──────────────┐
                  │ MySQL Server │
                  │   Port 3306  │
                  └──────────────┘
                         │
                  ┌──────────────┐
                  │  phpMyAdmin  │
                  │   Port 8080  │
                  └──────────────┘

         ┌──────────────────┐
         │React Frontend    │
         │   Port 3000      │
         └──────────────────┘
```

### Core Services

#### 1. **API Gateway**
- **Technology**: Ocelot
- **Port**: 5000
- **Purpose**: Single entry point for all client requests, routing to appropriate microservices
- **Features**:
  - Request routing
  - Load balancing capability
  - CORS handling
  - Centralized logging point

#### 2. **Auth Service**
- **Port**: 5001
- **Database**: dressed_auth
- **Purpose**: Handle user authentication and authorization
- **Features**:
  - User registration (Designers & Suppliers)
  - JWT token generation
  - Login/Logout
  - Password hashing with BCrypt
  - Token validation

#### 3. **Design Service**
- **Port**: 5002
- **Database**: dressed_design
- **Purpose**: Manage design submissions and lifecycle
- **Features**:
  - Create design posts with images/PDFs
  - Categorize designs (Men/Women/Boy/Girl/Unisex)
  - View all designs or filter by category
  - Track design status
  - Designer portfolio management

#### 4. **Quote Service**
- **Port**: 5003
- **Database**: dressed_quote
- **Purpose**: Handle supplier quotations
- **Features**:
  - Suppliers submit quotes for designs
  - View quotes by design or supplier
  - Quote status management
  - Price and delivery time tracking
  - Terms and conditions handling

#### 5. **Order Service** (To be implemented)
- **Purpose**: Manage order lifecycle
- **Features**:
  - Order placement
  - Order tracking
  - Shipping notifications
  - Order status updates

#### 6. **Communication Service** (To be implemented)
- **Purpose**: Enable messaging between designers and suppliers
- **Features**:
  - Real-time messaging
  - Negotiation threads
  - Message history

#### 7. **Notification Service** (To be implemented)
- **Purpose**: Notify suppliers about new designs in subscribed categories
- **Features**:
  - Category-based subscriptions
  - Email/Push notifications
  - Notification preferences

### Database Design

#### Database Strategy
- **Database Per Service Pattern**: Each microservice has its own database
- **Database Engine**: MySQL 8.0
- **Management**: phpMyAdmin (accessible at http://localhost:8080)

#### Databases:
- `dressed_auth`: Users, Designers, Suppliers, Categories
- `dressed_design`: Designs
- `dressed_quote`: Quotes
- `dressed_order`: Orders
- `dressed_communication`: Messages

### Technology Stack

#### Backend
- **Language**: C# (.NET 6.0)
- **Framework**: ASP.NET Core Web API
- **Authentication**: JWT (JSON Web Tokens)
- **Password Hashing**: BCrypt.Net
- **API Gateway**: Ocelot
- **Database**: MySQL 8.0
- **ORM**: ADO.NET with MySql.Data

#### Frontend
- **Framework**: React 18
- **Build Tool**: Vite
- **Package Manager**: npm

#### DevOps
- **Containerization**: Docker
- **Orchestration**: Docker Compose
- **Database Management**: phpMyAdmin

### API Endpoints

#### Auth Service (via Gateway: http://localhost:5000)

```
POST   /api/auth/register     - Register new user (designer/supplier)
POST   /api/auth/login        - Login user
POST   /api/auth/validate     - Validate JWT token
GET    /api/auth/health       - Health check
```

#### Design Service (via Gateway: http://localhost:5000)

```
POST   /api/designs                    - Create new design (Designer only)
GET    /api/designs/{id}               - Get specific design
GET    /api/designs                    - Get all designs (with optional category filter)
GET    /api/designs/designer/{id}      - Get designs by designer
PATCH  /api/designs/{id}/status        - Update design status
DELETE /api/designs/{id}               - Delete design (Designer only)
GET    /api/designs/health             - Health check
```

#### Quote Service (via Gateway: http://localhost:5000)

```
POST   /api/quotes                     - Create quote (Supplier only)
GET    /api/quotes/{id}                - Get specific quote
GET    /api/quotes/design/{designId}   - Get quotes for a design
GET    /api/quotes/supplier/{supplierId} - Get quotes by supplier
PATCH  /api/quotes/{id}/status         - Update quote status
DELETE /api/quotes/{id}                - Delete quote (Supplier only)
GET    /api/quotes/health              - Health check
```

### Data Models

#### User Types
- **Designer**: Can post designs, view quotes, place orders
- **Supplier**: Can view designs, submit quotes, manage orders
- **Admin**: Platform management (future)

#### Clothing Categories
- Men
- Women
- Boy
- Girl
- Unisex

#### Design Status Flow
```
Draft → Published → QuotingOpen → QuotingClosed → Ordered → Completed
                                                 ↓
                                            Cancelled
```

#### Quote Status Flow
```
Submitted → UnderNegotiation → Accepted
                             ↓
                           Rejected
                             ↓
                           Expired
```

#### Order Status Flow
```
Placed → Confirmed → InProduction → Shipped → Delivered
                                   ↓
                              Cancelled/Disputed
```

### Setup & Deployment

#### Prerequisites
- Docker Desktop
- Docker Compose
- .NET 6 SDK (for local development)
- Node.js 18+ (for frontend development)

#### Quick Start

1. **Clone the repository**
```bash
cd "DressedCore Backend"
```

2. **Build and run with Docker Compose**
```bash
docker-compose up --build
```

3. **Access the services**
- API Gateway: http://localhost:5000
- Auth Service: http://localhost:5001
- Design Service: http://localhost:5002
- Quote Service: http://localhost:5003
- phpMyAdmin: http://localhost:8080
  - Server: mysql
  - Username: root
  - Password: rootpassword
- Frontend: http://localhost:3000

#### Sample Credentials

**Designer Account**:
- Email: designer@example.com
- Password: designer123

**Supplier Account**:
- Email: supplier@example.com
- Password: supplier123

### Development Workflow

#### Local Development (Without Docker)

1. **Start MySQL**
```bash
# Using XAMPP or local MySQL installation
# Import database/init.sql
```

2. **Update connection strings in appsettings.json**
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=dressed_auth;User=root;Password=yourpassword;"
}
```

3. **Run services**
```bash
cd "src/Services/Auth.Service"
dotnet run

cd "src/Services/Design.Service"
dotnet run

cd "src/Services/Quote.Service"
dotnet run

cd "src/Gateway/ApiGateway"
dotnet run
```

### Deployment Strategy

#### Recommended Cloud Platform: Azure

**Azure Services to Use**:
1. **Azure Kubernetes Service (AKS)**: Container orchestration
2. **Azure Database for MySQL**: Managed database
3. **Azure Container Registry**: Store Docker images
4. **Azure API Management**: Enhanced API gateway features
5. **Azure Application Insights**: Monitoring and logging
6. **Azure Key Vault**: Secrets management

#### Deployment Steps

1. **Build and Push Docker Images**
```bash
docker build -t <registry>/auth-service:v1 -f src/Services/Auth.Service/Dockerfile .
docker push <registry>/auth-service:v1
```

2. **Deploy to AKS**
```bash
kubectl apply -f k8s/deployment.yaml
kubectl apply -f k8s/service.yaml
```

3. **Configure Ingress**
```bash
kubectl apply -f k8s/ingress.yaml
```

#### Scalability Considerations

- **Horizontal Scaling**: Each microservice can be scaled independently
- **Database Connection Pooling**: Optimize database connections
- **Caching**: Implement Redis for frequently accessed data
- **Message Queue**: Use Azure Service Bus for async communication
- **Load Balancing**: Use Azure Load Balancer or Application Gateway

#### Reliability & High Availability

- **Health Checks**: Each service exposes /health endpoint
- **Circuit Breakers**: Implement Polly for fault tolerance
- **Retry Policies**: Automatic retry for transient failures
- **Database Replication**: MySQL read replicas
- **Backup Strategy**: Automated daily backups
- **Monitoring**: Azure Monitor + Application Insights

### Security Best Practices

1. **Authentication & Authorization**
   - JWT tokens with expiration
   - Role-based access control (RBAC)
   - HTTPS in production

2. **Data Protection**
   - Password hashing with BCrypt
   - SQL injection prevention with parameterized queries
   - Input validation

3. **API Security**
   - Rate limiting
   - CORS configuration
   - API key authentication for service-to-service

4. **Secrets Management**
   - Use Azure Key Vault in production
   - Never commit secrets to Git
   - Environment-based configuration

### Testing Strategy

1. **Unit Tests**: Test business logic in isolation
2. **Integration Tests**: Test API endpoints
3. **Load Tests**: Use Azure Load Testing
4. **Security Tests**: OWASP ZAP scanning

### Monitoring & Logging

1. **Application Logging**: Structured logging with Serilog
2. **Performance Metrics**: Track API response times
3. **Error Tracking**: Centralized error logging
4. **Dashboards**: Grafana/Application Insights dashboards

### Future Enhancements

1. **Payment Integration**: Stripe/PayPal integration
2. **File Storage**: Azure Blob Storage for design files
3. **Real-time Notifications**: SignalR for live updates
4. **Search Engine**: Elasticsearch for advanced search
5. **Analytics**: Designer/Supplier dashboards
6. **Mobile Apps**: React Native applications
7. **AI/ML**: Design recommendation engine

### Contributing

1. Fork the repository
2. Create a feature branch
3. Commit changes
4. Push to the branch
5. Create a Pull Request

### License

Proprietary - Commercial in confidence

### Support

For issues and questions, contact: support@dressed.com

---

**Built with ❤️ for CreditSource Assignment 2025**
