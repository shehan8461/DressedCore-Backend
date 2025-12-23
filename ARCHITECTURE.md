# Dressed™ - Architecture & Deployment Documentation

## System Architecture Diagram

### High-Level Architecture

```
┌────────────────────────────────────────────────────────────────────┐
│                           CLIENT LAYER                              │
│                                                                      │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐   │
│  │  Designer Web   │  │  Supplier Web   │  │   Admin Web     │   │
│  │   Application   │  │   Application   │  │   Application   │   │
│  │   (React App)   │  │   (React App)   │  │   (React App)   │   │
│  └────────┬────────┘  └────────┬────────┘  └────────┬────────┘   │
│           └────────────────┬────────────────────────┘              │
│                            │ HTTPS/REST                            │
└────────────────────────────┼──────────────────────────────────────┘
                             ▼
┌────────────────────────────────────────────────────────────────────┐
│                         API GATEWAY LAYER                           │
│                                                                      │
│              ┌──────────────────────────────────┐                  │
│              │     Ocelot API Gateway          │                  │
│              │     (Port 5000)                  │                  │
│              │  - Routing                       │                  │
│              │  - Load Balancing                │                  │
│              │  - Rate Limiting                 │                  │
│              └──────────┬───────────────────────┘                  │
└─────────────────────────┼──────────────────────────────────────────┘
                          │
         ┌────────────────┼────────────────┐
         │                │                │
         ▼                ▼                ▼
┌────────────────────────────────────────────────────────────────────┐
│                      MICROSERVICES LAYER                            │
│                                                                      │
│  ┌───────────────┐  ┌───────────────┐  ┌───────────────┐         │
│  │ Auth Service  │  │Design Service │  │Quote Service  │         │
│  │  Port: 5001   │  │  Port: 5002   │  │  Port: 5003   │         │
│  │               │  │               │  │               │         │
│  │ - Register    │  │ - Create      │  │ - Submit      │         │
│  │ - Login       │  │ - List        │  │ - View        │         │
│  │ - JWT Token   │  │ - Update      │  │ - Accept      │         │
│  └───────┬───────┘  └───────┬───────┘  └───────┬───────┘         │
│          │                  │                  │                   │
│  ┌───────────────┐  ┌───────────────┐  ┌───────────────┐         │
│  │ Order Service │  │  Comm Service │  │  Notif Service│         │
│  │  Port: 5004   │  │  Port: 5005   │  │  Port: 5006   │         │
│  │               │  │               │  │               │         │
│  │ - Place Order │  │ - Messaging   │  │ - Notify      │         │
│  │ - Track       │  │ - Negotiate   │  │ - Subscribe   │         │
│  │ - Shipping    │  │ - History     │  │ - Email/Push  │         │
│  └───────┬───────┘  └───────┬───────┘  └───────┬───────┘         │
│          │                  │                  │                   │
└──────────┼──────────────────┼──────────────────┼───────────────────┘
           │                  │                  │
           └──────────────────┼──────────────────┘
                              ▼
┌────────────────────────────────────────────────────────────────────┐
│                       DATA PERSISTENCE LAYER                        │
│                                                                      │
│              ┌──────────────────────────────────┐                  │
│              │        MySQL Server 8.0          │                  │
│              │        Port: 3306                │                  │
│              │                                  │                  │
│              │  Databases:                      │                  │
│              │  ├─ dressed_auth                 │                  │
│              │  ├─ dressed_design               │                  │
│              │  ├─ dressed_quote                │                  │
│              │  ├─ dressed_order                │                  │
│              │  └─ dressed_communication        │                  │
│              │                                  │                  │
│              │  Management: phpMyAdmin (8080)   │                  │
│              └──────────────────────────────────┘                  │
└────────────────────────────────────────────────────────────────────┘

┌────────────────────────────────────────────────────────────────────┐
│                     INFRASTRUCTURE LAYER                            │
│                                                                      │
│  ┌──────────────────────────────────────────────────────────────┐ │
│  │              Docker & Docker Compose                          │ │
│  │  - Container Orchestration                                    │ │
│  │  - Network Management                                         │ │
│  │  - Volume Management                                          │ │
│  └──────────────────────────────────────────────────────────────┘ │
└────────────────────────────────────────────────────────────────────┘
```

## Microservices Details

### 1. Auth Service
**Purpose**: User authentication and authorization
**Port**: 5001
**Database**: dressed_auth

**Key Features**:
- User registration (Designers & Suppliers)
- Login with JWT tokens
- Password hashing with BCrypt
- Token validation
- Role-based access control

**Endpoints**:
```
POST /api/auth/register
POST /api/auth/login
POST /api/auth/validate
GET  /api/auth/health
```

### 2. Design Service
**Purpose**: Design submission and management
**Port**: 5002
**Database**: dressed_design

**Key Features**:
- Create design posts with files
- Categorize by clothing type
- View all designs or filter
- Track design status
- Designer portfolio

**Endpoints**:
```
POST   /api/designs
GET    /api/designs/{id}
GET    /api/designs
GET    /api/designs/designer/{id}
PATCH  /api/designs/{id}/status
DELETE /api/designs/{id}
```

### 3. Quote Service
**Purpose**: Supplier quotation management
**Port**: 5003
**Database**: dressed_quote

**Key Features**:
- Submit quotes for designs
- View quotes by design/supplier
- Quote status tracking
- Negotiation support

**Endpoints**:
```
POST   /api/quotes
GET    /api/quotes/{id}
GET    /api/quotes/design/{designId}
GET    /api/quotes/supplier/{supplierId}
PATCH  /api/quotes/{id}/status
```

### 4. Order Service (Future)
**Purpose**: Order lifecycle management
**Port**: 5004
**Database**: dressed_order

### 5. Communication Service (Future)
**Purpose**: Messaging between users
**Port**: 5005
**Database**: dressed_communication

### 6. Notification Service (Future)
**Purpose**: Alert suppliers about new designs
**Port**: 5006

## Data Flow Diagrams

### Designer Workflow
```
┌─────────┐
│Designer │
└────┬────┘
     │
     │ 1. Register/Login
     ▼
┌─────────────┐
│Auth Service │
└─────┬───────┘
      │ JWT Token
      ▼
┌─────────────┐
│   Gateway   │
└─────┬───────┘
      │ 2. Submit Design
      ▼
┌──────────────┐
│Design Service│───────► MySQL (dressed_design)
└──────┬───────┘
       │ 3. Design Created
       ▼
┌──────────────────┐
│Notification Svc  │───► Notify Suppliers
└──────────────────┘
       │
       │ 4. View Quotes
       ▼
┌─────────────┐
│Quote Service│◄──── MySQL (dressed_quote)
└─────────────┘
       │
       │ 5. Accept Quote
       ▼
┌─────────────┐
│Order Service│───► MySQL (dressed_order)
└─────────────┘
```

### Supplier Workflow
```
┌─────────┐
│Supplier │
└────┬────┘
     │
     │ 1. Register/Login + Subscribe to Categories
     ▼
┌─────────────┐
│Auth Service │
└─────┬───────┘
      │ JWT Token
      ▼
┌──────────────────┐
│Notification Svc  │◄─── New Design Alert
└──────┬───────────┘
       │
       │ 2. View Design
       ▼
┌──────────────┐
│Design Service│◄──── MySQL (dressed_design)
└──────────────┘
       │
       │ 3. Submit Quote
       ▼
┌─────────────┐
│Quote Service│───► MySQL (dressed_quote)
└─────┬───────┘
      │
      │ 4. Negotiate via Messages
      ▼
┌──────────────────┐
│Communication Svc │───► MySQL (dressed_communication)
└──────────────────┘
      │
      │ 5. Quote Accepted
      ▼
┌─────────────┐
│Order Service│───► Fulfill Order
└─────────────┘
```

## Database Schema

### dressed_auth Database
```sql
Users
- Id (PK)
- Email (Unique)
- PasswordHash
- FirstName
- LastName
- UserType (1=Designer, 2=Supplier, 3=Admin)
- CreatedAt
- IsActive

Designers
- Id (PK)
- UserId (FK → Users)
- CompanyName
- ContactNumber
- Address
- Website
- Rating
- CreatedAt

Suppliers
- Id (PK)
- UserId (FK → Users)
- CompanyName
- ContactNumber
- Address
- ManufacturingCapabilities
- Rating
- CreatedAt

SupplierCategories
- Id (PK)
- SupplierId (FK → Suppliers)
- Category (1=Men, 2=Women, 3=Boy, 4=Girl, 5=Unisex)
```

### dressed_design Database
```sql
Designs
- Id (PK)
- DesignerId (FK)
- Title
- Description
- Category
- FileUrls (JSON)
- Status
- CreatedAt
- Deadline
- Quantity
- Specifications
```

### dressed_quote Database
```sql
Quotes
- Id (PK)
- DesignId (FK)
- SupplierId (FK)
- Price
- Currency
- DeliveryTimeInDays
- QuoteText
- TermsAndConditions
- Status
- CreatedAt
- UpdatedAt
```

## Deployment Architecture (Azure)

```
┌────────────────────────────────────────────────────────────────┐
│                      Azure Cloud                               │
│                                                                  │
│  ┌─────────────────────────────────────────────────────────┐  │
│  │             Azure Traffic Manager                        │  │
│  │             (Global Load Balancing)                      │  │
│  └─────────────────┬───────────────────────────────────────┘  │
│                    │                                            │
│  ┌─────────────────▼───────────────────────────────────────┐  │
│  │        Azure Application Gateway                         │  │
│  │        (Web Application Firewall)                        │  │
│  └─────────────────┬───────────────────────────────────────┘  │
│                    │                                            │
│  ┌─────────────────▼───────────────────────────────────────┐  │
│  │     Azure Kubernetes Service (AKS)                       │  │
│  │                                                           │  │
│  │  ┌──────────┐  ┌──────────┐  ┌──────────┐              │  │
│  │  │Auth Pod  │  │Design Pod│  │Quote Pod │              │  │
│  │  │(Replica:3)│  │(Replica:3)│  │(Replica:3)│              │  │
│  │  └──────────┘  └──────────┘  └──────────┘              │  │
│  │                                                           │  │
│  │  ┌──────────┐  ┌──────────┐  ┌──────────┐              │  │
│  │  │Order Pod │  │Comm Pod  │  │Notif Pod │              │  │
│  │  └──────────┘  └──────────┘  └──────────┘              │  │
│  └───────────────────────────────────────────────────────────┘  │
│                    │                                            │
│  ┌─────────────────▼───────────────────────────────────────┐  │
│  │      Azure Database for MySQL (Flexible Server)         │  │
│  │      - High Availability                                 │  │
│  │      - Automated Backups                                 │  │
│  │      - Read Replicas                                     │  │
│  └──────────────────────────────────────────────────────────┘  │
│                                                                  │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │              Azure Blob Storage                           │  │
│  │              (Design Files: Images, PDFs)                 │  │
│  └──────────────────────────────────────────────────────────┘  │
│                                                                  │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │              Azure Key Vault                              │  │
│  │              (Secrets, Connection Strings, Keys)          │  │
│  └──────────────────────────────────────────────────────────┘  │
│                                                                  │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │       Azure Application Insights                          │  │
│  │       (Monitoring, Logging, Telemetry)                    │  │
│  └──────────────────────────────────────────────────────────┘  │
│                                                                  │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │       Azure Redis Cache                                   │  │
│  │       (Session Management, Caching)                       │  │
│  └──────────────────────────────────────────────────────────┘  │
│                                                                  │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │       Azure Service Bus                                   │  │
│  │       (Async Messaging Between Services)                  │  │
│  └──────────────────────────────────────────────────────────┘  │
└────────────────────────────────────────────────────────────────┘
```

## Security Architecture

```
┌──────────────────────────────────────────────────────────────┐
│                    Security Layers                           │
│                                                                │
│  Layer 1: Network Security                                    │
│  ├─ Azure Firewall                                            │
│  ├─ Network Security Groups (NSG)                             │
│  └─ DDoS Protection                                           │
│                                                                │
│  Layer 2: Application Security                                │
│  ├─ Web Application Firewall (WAF)                            │
│  ├─ API Rate Limiting                                         │
│  └─ CORS Policy                                               │
│                                                                │
│  Layer 3: Identity & Access                                   │
│  ├─ JWT Token Authentication                                  │
│  ├─ Role-Based Access Control                                 │
│  └─ Multi-Factor Authentication (Future)                      │
│                                                                │
│  Layer 4: Data Protection                                     │
│  ├─ TLS/SSL Encryption in Transit                             │
│  ├─ Encryption at Rest                                        │
│  ├─ Password Hashing (BCrypt)                                 │
│  └─ SQL Injection Prevention                                  │
│                                                                │
│  Layer 5: Secrets Management                                  │
│  ├─ Azure Key Vault                                           │
│  ├─ Environment Variables                                     │
│  └─ No Hardcoded Secrets                                      │
│                                                                │
│  Layer 6: Monitoring & Compliance                             │
│  ├─ Application Insights Logging                              │
│  ├─ Audit Logs                                                │
│  └─ Compliance Scanning                                       │
└──────────────────────────────────────────────────────────────┘
```

## Cost Estimation (Azure - Monthly)

```
Service                          Tier              Cost (USD/month)
─────────────────────────────────────────────────────────────────
AKS Cluster (3 nodes)           Standard D2s v3   ~$210
Azure Database for MySQL        General Purpose   ~$150
Azure Blob Storage              Standard          ~$20
Azure Application Gateway       Standard V2       ~$250
Azure Key Vault                 Standard          ~$5
Application Insights            Pay-as-you-go     ~$50
Azure Redis Cache               Basic             ~$15
Traffic Manager                 Standard          ~$5
Azure Service Bus               Standard          ~$10
                                                  ──────
Total Estimated Cost                              ~$715/month
```

## Performance & Scalability

### Auto-Scaling Configuration
```yaml
# Horizontal Pod Autoscaler
minReplicas: 2
maxReplicas: 10
targetCPUUtilizationPercentage: 70
targetMemoryUtilizationPercentage: 80
```

### Expected Performance
- **API Response Time**: < 200ms (p95)
- **Throughput**: ~1000 requests/second per service
- **Concurrent Users**: ~10,000
- **Database Connections**: Pooled (max 100 per service)

## Disaster Recovery

### Backup Strategy
- **Database Backups**: Daily automated backups, 30-day retention
- **Point-in-Time Restore**: Available for last 7 days
- **Geo-Redundant Storage**: Replicated across regions
- **Application State**: Stateless services for easy recovery

### Recovery Time Objective (RTO)
- **Critical Services**: < 1 hour
- **Non-Critical Services**: < 4 hours

### Recovery Point Objective (RPO)
- **Database**: < 5 minutes
- **File Storage**: < 15 minutes

---

## Contact & Support

**Project**: Dressed™ Fashion Platform
**Assignment**: CreditSource 2025
**Documentation Version**: 1.0
**Last Updated**: December 2025
