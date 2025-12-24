# Dressed™ - System Architecture & Workflow Diagrams

## Table of Contents
1. [High-Level System Architecture](#high-level-system-architecture)
2. [Designer Workflow](#designer-workflow)
3. [Supplier Workflow](#supplier-workflow)
4. [Technical Flow Diagrams](#technical-flow-diagrams)
5. [Database Architecture](#database-architecture)

---

## High-Level System Architecture

```
┌─────────────────────────────────────────────────────────────────────────┐
│                           DRESSED™ PLATFORM                              │
└─────────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────────┐
│                          CLIENT LAYER                                    │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                           │
│  ┌──────────────────┐        ┌──────────────────┐                       │
│  │  Designer Portal │        │  Supplier Portal │                       │
│  │   (React SPA)    │        │   (React SPA)    │                       │
│  │  Port: 3000      │        │  Port: 3000      │                       │
│  └────────┬─────────┘        └────────┬─────────┘                       │
│           │                            │                                 │
│           └────────────┬───────────────┘                                 │
│                        │                                                 │
└────────────────────────┼─────────────────────────────────────────────────┘
                         │
                         │ HTTPS/REST
                         │
┌────────────────────────▼─────────────────────────────────────────────────┐
│                       API GATEWAY LAYER                                  │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                           │
│              ┌────────────────────────────────┐                          │
│              │      API GATEWAY (Ocelot)      │                          │
│              │        Port: 4000              │                          │
│              │  - Route Management            │                          │
│              │  - Load Balancing              │                          │
│              │  - Authentication              │                          │
│              │  - Rate Limiting               │                          │
│              └───────────┬────────────────────┘                          │
│                          │                                               │
└──────────────────────────┼───────────────────────────────────────────────┘
                           │
         ┌─────────────────┼─────────────────┐
         │                 │                 │
┌────────▼────────┐ ┌──────▼──────┐ ┌───────▼────────┐
│                 │ │             │ │                │
│  AUTH SERVICE   │ │   DESIGN    │ │  QUOTE SERVICE │
│   Port: 5001    │ │   SERVICE   │ │   Port: 5003   │
│                 │ │  Port: 5002 │ │                │
│ ┌─────────────┐ │ │ ┌─────────┐ │ │ ┌────────────┐ │
│ │ Controllers │ │ │ │Controllers│ │ │ │Controllers │ │
│ │   Business  │ │ │ │ Business │ │ │ │  Business  │ │
│ │    Logic    │ │ │ │  Logic   │ │ │ │   Logic    │ │
│ │ Data Access │ │ │ │Data Access│ │ │ │Data Access │ │
│ └──────┬──────┘ │ │ └────┬────┘ │ │ └─────┬──────┘ │
└────────┼────────┘ └──────┼──────┘ └───────┼────────┘
         │                 │                 │
┌────────▼─────────────────▼─────────────────▼────────┐
│              DATABASE LAYER (MySQL 8.0)              │
├──────────────────────────────────────────────────────┤
│                                                      │
│  ┌─────────────┐  ┌──────────────┐  ┌────────────┐ │
│  │ dressed_auth│  │dressed_design│  │dressed_quote│ │
│  │             │  │              │  │            │ │
│  │ - Users     │  │ - Designs    │  │ - Quotes   │ │
│  │ - Designers │  │ - Designers  │  │ - Suppliers│ │
│  │ - Suppliers │  │              │  │            │ │
│  └─────────────┘  └──────────────┘  └────────────┘ │
│                                                      │
│         phpMyAdmin (Port: 8080)                      │
└──────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────┐
│            INFRASTRUCTURE LAYER                      │
├──────────────────────────────────────────────────────┤
│                                                      │
│  ┌────────────────────────────────────────────────┐ │
│  │          Docker Container Orchestration        │ │
│  │               (Docker Compose)                 │ │
│  │                                                │ │
│  │  - Container Management                        │ │
│  │  - Service Discovery                           │ │
│  │  - Health Monitoring                           │ │
│  │  - Volume Management                           │ │
│  │  - Network Isolation                           │ │
│  └────────────────────────────────────────────────┘ │
│                                                      │
└──────────────────────────────────────────────────────┘
```

### Architecture Components

#### **1. Client Layer**
- **Technology**: React 18 with Vite
- **Features**: 
  - Single Page Application (SPA)
  - Responsive design
  - JWT token management
  - Role-based UI rendering

#### **2. API Gateway**
- **Technology**: Ocelot (.NET 6)
- **Responsibilities**:
  - Centralized API routing
  - Authentication/Authorization
  - Request/Response transformation
  - CORS handling
  - Rate limiting

#### **3. Microservices Layer**
- **Auth Service**: User authentication and authorization
- **Design Service**: Design management and listings
- **Quote Service**: Quote submission and management
- **Future Services**: Order, Communication, Notification

#### **4. Database Layer**
- **Strategy**: Database-per-service pattern
- **Technology**: MySQL 8.0
- **Management**: phpMyAdmin

#### **5. Infrastructure**
- **Containerization**: Docker & Docker Compose
- **Orchestration**: Docker Compose
- **Networking**: Custom bridge network

---

## Designer Workflow

### Complete Designer Journey

```
┌─────────────────────────────────────────────────────────────────────┐
│                    DESIGNER WORKFLOW DIAGRAM                         │
└─────────────────────────────────────────────────────────────────────┘

START
  │
  ▼
┌─────────────────────┐
│  1. REGISTRATION    │
│  ─────────────────  │
│  • Visit platform   │
│  • Select "Designer"│
│  • Enter details:   │
│    - Email          │
│    - Password       │
│    - Company Name   │
│    - Contact Info   │
│  • Submit           │
└──────────┬──────────┘
           │
           │ API: POST /api/auth/register
           │ Body: { userType: 1 (Designer) }
           ▼
┌─────────────────────┐
│  Account Created    │
│  JWT Token Issued   │
└──────────┬──────────┘
           │
           ▼
┌─────────────────────┐
│    2. LOGIN         │
│  ─────────────────  │
│  • Enter email      │
│  • Enter password   │
│  • Click Login      │
└──────────┬──────────┘
           │
           │ API: POST /api/auth/login
           │ Returns: JWT Token
           ▼
┌─────────────────────┐
│  3. DASHBOARD       │
│  ─────────────────  │
│  View Options:      │
│  • My Designs       │
│  • Create New       │
│  • View Quotes      │
│  • Profile          │
└──────────┬──────────┘
           │
           ▼
┌─────────────────────┐
│ 4. CREATE DESIGN    │
│  ─────────────────  │
│  Fill Form:         │
│  • Title            │
│  • Description      │
│  • Category         │
│    (Men/Women/      │
│     Boy/Girl/       │
│     Unisex)         │
│  • File URLs        │
│  • Quantity         │
│  • Specifications   │
│  • Deadline (Opt)   │
│  • Submit           │
└──────────┬──────────┘
           │
           │ API: POST /api/designs
           │ Headers: Authorization: Bearer {token}
           ▼
┌─────────────────────┐
│  Design Published   │
│  Status: Published  │
└──────────┬──────────┘
           │
           │ Automatic notification to
           │ matching suppliers
           ▼
┌─────────────────────┐
│ 5. VIEW MY DESIGNS  │
│  ─────────────────  │
│  Display Grid:      │
│  • All Designs      │
│  • Filter by Status │
│    - Published      │
│    - Quoting Open   │
│    - Ordered        │
│  • Quote Count      │
│  • Created Date     │
└──────────┬──────────┘
           │
           │ API: GET /api/designs/designer/{id}
           ▼
┌─────────────────────┐
│ 6. RECEIVE QUOTES   │
│  ─────────────────  │
│  Suppliers submit   │
│  quotes for design  │
│                     │
│  Designer receives: │
│  • Quote price      │
│  • Delivery time    │
│  • Terms            │
│  • Supplier info    │
└──────────┬──────────┘
           │
           ▼
┌─────────────────────┐
│ 7. VIEW QUOTES      │
│  ─────────────────  │
│  For each design:   │
│  • List all quotes  │
│  • Compare prices   │
│  • Compare delivery │
│  • Check ratings    │
│  • Read terms       │
└──────────┬──────────┘
           │
           │ API: GET /api/quotes/design/{designId}
           ▼
┌─────────────────────┐
│ 8. EVALUATE QUOTES  │
│  ─────────────────  │
│  Compare:           │
│  • Price            │
│  • Delivery Time    │
│  • Supplier Rating  │
│  • Terms            │
│  • Past Performance │
└──────────┬──────────┘
           │
           ▼
┌─────────────────────┐
│ 9. ACCEPT QUOTE     │
│  ─────────────────  │
│  • Select best quote│
│  • Click "Accept"   │
│  • Confirm order    │
└──────────┬──────────┘
           │
           │ API: PATCH /api/quotes/{id}/status
           │ Body: { status: "Accepted" }
           ▼
┌─────────────────────┐
│ 10. PLACE ORDER     │
│  ─────────────────  │
│  • Review details   │
│  • Confirm payment  │
│  • Submit order     │
└──────────┬──────────┘
           │
           │ API: POST /api/orders
           │ (Future implementation)
           ▼
┌─────────────────────┐
│ 11. TRACK ORDER     │
│  ─────────────────  │
│  Monitor:           │
│  • Order status     │
│  • Production stage │
│  • Shipping details │
│  • Expected delivery│
└──────────┬──────────┘
           │
           ▼
┌─────────────────────┐
│ 12. RECEIVE ORDER   │
│  ─────────────────  │
│  • Confirm receipt  │
│  • Quality check    │
│  • Rate supplier    │
│  • Leave review     │
└──────────┬──────────┘
           │
           ▼
         END
```

### Designer Key Features

| Feature | Description | Status |
|---------|-------------|--------|
| Design Upload | Submit designs with images/PDFs | ✅ Implemented |
| Quote Management | View and compare supplier quotes | ✅ Implemented |
| Order Tracking | Monitor production and shipping | ⏳ Planned |
| Supplier Ratings | Rate and review suppliers | ⏳ Planned |
| Messaging | Direct communication with suppliers | ⏳ Planned |
| Analytics | View design performance metrics | ⏳ Planned |

---

## Supplier Workflow

### Complete Supplier Journey

```
┌─────────────────────────────────────────────────────────────────────┐
│                    SUPPLIER WORKFLOW DIAGRAM                         │
└─────────────────────────────────────────────────────────────────────┘

START
  │
  ▼
┌─────────────────────┐
│  1. REGISTRATION    │
│  ─────────────────  │
│  • Visit platform   │
│  • Select "Supplier"│
│  • Enter details:   │
│    - Email          │
│    - Password       │
│    - Company Name   │
│    - Contact Info   │
│    - Manufacturing  │
│      Capabilities   │
│  • Select Categories│
│    (Men/Women/      │
│     Boy/Girl/       │
│     Unisex)         │
│  • Submit           │
└──────────┬──────────┘
           │
           │ API: POST /api/auth/register
           │ Body: { userType: 2 (Supplier), 
           │         categories: [1,2,5] }
           ▼
┌─────────────────────┐
│  Account Created    │
│  JWT Token Issued   │
│  Categories Saved   │
└──────────┬──────────┘
           │
           ▼
┌─────────────────────┐
│    2. LOGIN         │
│  ─────────────────  │
│  • Enter email      │
│  • Enter password   │
│  • Click Login      │
└──────────┬──────────┘
           │
           │ API: POST /api/auth/login
           │ Returns: JWT Token
           ▼
┌─────────────────────┐
│  3. DASHBOARD       │
│  ─────────────────  │
│  View Options:      │
│  • Browse Designs   │
│  • My Quotes        │
│  • Active Orders    │
│  • Profile          │
└──────────┬──────────┘
           │
           ▼
┌─────────────────────┐
│ 4. BROWSE DESIGNS   │
│  ─────────────────  │
│  View:              │
│  • All published    │
│    designs          │
│  • Filter by:       │
│    - Category       │
│    - Quantity       │
│    - Deadline       │
│  • Search           │
│  • Sort by date     │
└──────────┬──────────┘
           │
           │ API: GET /api/designs?category={cat}
           ▼
┌─────────────────────┐
│ 5. VIEW DESIGN      │
│    DETAILS          │
│  ─────────────────  │
│  See:               │
│  • Full description │
│  • Design files     │
│  • Specifications   │
│  • Quantity needed  │
│  • Deadline         │
│  • Designer info    │
│  • Existing quotes  │
└──────────┬──────────┘
           │
           │ API: GET /api/designs/{id}
           ▼
┌─────────────────────┐
│ 6. EVALUATE DESIGN  │
│  ─────────────────  │
│  Assess:            │
│  • Complexity       │
│  • Material costs   │
│  • Production time  │
│  • Profit margin    │
│  • Capacity         │
│  • Risk factors     │
└──────────┬──────────┘
           │
           ▼
      ┌────┴────┐
      │ Decision│
      └────┬────┘
           │
    ┌──────┴──────┐
    │             │
    ▼             ▼
  Skip       Submit Quote
    │             │
    │             ▼
    │    ┌─────────────────────┐
    │    │ 7. SUBMIT QUOTE     │
    │    │  ─────────────────  │
    │    │  Fill Form:         │
    │    │  • Total Price      │
    │    │  • Currency (USD)   │
    │    │  • Delivery Time    │
    │    │    (in days)        │
    │    │  • Quote Text       │
    │    │    (Description)    │
    │    │  • Terms &          │
    │    │    Conditions       │
    │    │  • Submit           │
    │    └──────────┬──────────┘
    │               │
    │               │ API: POST /api/quotes
    │               │ Body: { designId, supplierId,
    │               │         price, deliveryTime... }
    │               ▼
    │    ┌─────────────────────┐
    │    │  Quote Submitted    │
    │    │  Status: Submitted  │
    │    │  Notification sent  │
    │    │  to Designer        │
    │    └──────────┬──────────┘
    │               │
    └───────────────┤
                    │
                    ▼
         ┌─────────────────────┐
         │ 8. VIEW MY QUOTES   │
         │  ─────────────────  │
         │  Display:           │
         │  • All quotes       │
         │  • Filter by:       │
         │    - Submitted      │
         │    - Accepted       │
         │    - Rejected       │
         │  • Design details   │
         │  • Status updates   │
         └──────────┬──────────┘
                    │
                    │ API: GET /api/quotes/supplier/{id}
                    ▼
         ┌─────────────────────┐
         │ 9. WAIT FOR DESIGNER│
         │    DECISION         │
         │  ─────────────────  │
         │  Designer reviews:  │
         │  • All quotes       │
         │  • Makes selection  │
         │                     │
         │  Possible outcomes: │
         │  • Accepted ✓       │
         │  • Rejected ✗       │
         │  • Pending...       │
         └──────────┬──────────┘
                    │
                    ▼
              ┌─────┴─────┐
              │  Status?  │
              └─────┬─────┘
                    │
         ┌──────────┼──────────┐
         │          │          │
         ▼          ▼          ▼
    Rejected   Pending    Accepted
         │                     │
         │                     ▼
         │          ┌─────────────────────┐
         │          │ 10. QUOTE ACCEPTED  │
         │          │  ─────────────────  │
         │          │  Receive:           │
         │          │  • Order details    │
         │          │  • Payment terms    │
         │          │  • Delivery address │
         │          │  • Timeline         │
         │          └──────────┬──────────┘
         │                     │
         │                     │ API: Notification received
         │                     ▼
         │          ┌─────────────────────┐
         │          │ 11. CONFIRM ORDER   │
         │          │  ─────────────────  │
         │          │  • Acknowledge      │
         │          │  • Confirm timeline │
         │          │  • Accept terms     │
         │          │  • Start production │
         │          └──────────┬──────────┘
         │                     │
         │                     │ API: PATCH /api/orders/{id}/status
         │                     ▼
         │          ┌─────────────────────┐
         │          │ 12. PRODUCTION      │
         │          │  ─────────────────  │
         │          │  Update status:     │
         │          │  • Materials        │
         │          │    acquired         │
         │          │  • In production    │
         │          │  • Quality check    │
         │          │  • Ready to ship    │
         │          └──────────┬──────────┘
         │                     │
         │                     ▼
         │          ┌─────────────────────┐
         │          │ 13. SHIPPING        │
         │          │  ─────────────────  │
         │          │  • Package order    │
         │          │  • Generate invoice │
         │          │  • Ship to designer │
         │          │  • Update tracking  │
         │          └──────────┬──────────┘
         │                     │
         │                     ▼
         │          ┌─────────────────────┐
         │          │ 14. DELIVERY        │
         │          │  ─────────────────  │
         │          │  • Confirm delivery │
         │          │  • Receive payment  │
         │          │  • Get feedback     │
         │          │  • Update rating    │
         │          └──────────┬──────────┘
         │                     │
         └─────────────────────┘
                               │
                               ▼
                             END
```

### Supplier Key Features

| Feature | Description | Status |
|---------|-------------|--------|
| Browse Designs | View available design projects | ✅ Implemented |
| Submit Quotes | Create competitive quotations | ✅ Implemented |
| Order Management | Manage accepted orders | ⏳ Planned |
| Production Tracking | Update production status | ⏳ Planned |
| Messaging | Communicate with designers | ⏳ Planned |
| Analytics | View quote success rates | ⏳ Planned |

---

## Technical Flow Diagrams

### Authentication Flow

```
┌──────────┐         ┌─────────────┐         ┌──────────────┐
│  Client  │         │ API Gateway │         │ Auth Service │
└────┬─────┘         └──────┬──────┘         └──────┬───────┘
     │                      │                        │
     │ POST /api/auth/login │                        │
     ├─────────────────────►│                        │
     │                      │ Forward with CORS      │
     │                      ├───────────────────────►│
     │                      │                        │
     │                      │                        │ Validate
     │                      │                        │ Credentials
     │                      │                        │
     │                      │                        │ Hash Password
     │                      │                        │ & Compare
     │                      │                        │
     │                      │   JWT Token Generated  │
     │                      │◄───────────────────────┤
     │   Token + User Info  │                        │
     │◄─────────────────────┤                        │
     │                      │                        │
     │ Store Token in       │                        │
     │ localStorage         │                        │
     │                      │                        │
     │                      │                        │
     │ Subsequent Requests  │                        │
     │ with Authorization   │                        │
     │ Header: Bearer Token │                        │
     ├─────────────────────►│                        │
     │                      │                        │
     │                      │ Validate Token         │
     │                      │                        │
     │                      │ Route to Service       │
     │                      ├───────────────────────►│
     │                      │                        │
     │     Response         │                        │
     │◄─────────────────────┤◄───────────────────────┤
     │                      │                        │
```

### Design Submission Flow

```
Designer                API Gateway           Design Service         Database
   │                         │                      │                   │
   │ Fill Design Form        │                      │                   │
   │ Click Submit            │                      │                   │
   ├────────────────────────►│                      │                   │
   │ POST /api/designs       │                      │                   │
   │ + JWT Token             │                      │                   │
   │                         │ Validate Token       │                   │
   │                         │ Forward Request      │                   │
   │                         ├─────────────────────►│                   │
   │                         │                      │ Verify Designer   │
   │                         │                      │ Role              │
   │                         │                      │                   │
   │                         │                      │ Parse Data        │
   │                         │                      │                   │
   │                         │                      │ INSERT Design     │
   │                         │                      ├──────────────────►│
   │                         │                      │                   │
   │                         │                      │ Design ID         │
   │                         │                      │◄──────────────────┤
   │                         │                      │                   │
   │                         │                      │ Fetch Full Design │
   │                         │                      ├──────────────────►│
   │                         │                      │                   │
   │                         │                      │ Design Data       │
   │                         │                      │◄──────────────────┤
   │                         │                      │                   │
   │                         │  Success Response    │                   │
   │                         │◄─────────────────────┤                   │
   │                         │  (Design Object)     │                   │
   │  Success + Design Data  │                      │                   │
   │◄────────────────────────┤                      │                   │
   │                         │                      │                   │
   │ Redirect to             │                      │                   │
   │ My Designs Page         │                      │                   │
   │                         │                      │                   │
```

### Quote Submission Flow

```
Supplier                 API Gateway           Quote Service          Database
   │                         │                      │                   │
   │ View Design Details     │                      │                   │
   │ Click "Submit Quote"    │                      │                   │
   │ Fill Quote Form         │                      │                   │
   ├────────────────────────►│                      │                   │
   │ POST /api/quotes        │                      │                   │
   │ + JWT Token             │                      │                   │
   │                         │ Validate Token       │                   │
   │                         │ Forward Request      │                   │
   │                         ├─────────────────────►│                   │
   │                         │                      │ Verify Supplier   │
   │                         │                      │ Role              │
   │                         │                      │                   │
   │                         │                      │ Validate Data     │
   │                         │                      │                   │
   │                         │                      │ INSERT Quote      │
   │                         │                      ├──────────────────►│
   │                         │                      │                   │
   │                         │                      │ Quote ID          │
   │                         │                      │◄──────────────────┤
   │                         │                      │                   │
   │                         │                      │ Send Notification │
   │                         │                      │ to Designer       │
   │                         │                      │ (Future Feature)  │
   │                         │                      │                   │
   │                         │  Success Response    │                   │
   │                         │◄─────────────────────┤                   │
   │                         │  (Quote Object)      │                   │
   │  Success + Quote Data   │                      │                   │
   │◄────────────────────────┤                      │                   │
   │                         │                      │                   │
   │ Show Success Message    │                      │                   │
   │                         │                      │                   │
```

---

## Database Architecture

### Entity Relationship Diagram

```
┌─────────────────────────────────────────────────────────────────────┐
│                      DATABASE: dressed_auth                          │
└─────────────────────────────────────────────────────────────────────┘

┌──────────────────┐
│      Users       │
├──────────────────┤
│ PK: Id           │
│    Email         │◄────────┐
│    PasswordHash  │         │
│    FirstName     │         │
│    LastName      │         │
│    UserType      │         │ 1:1
│    CreatedAt     │         │
│    IsActive      │         │
└──────────────────┘         │
                             │
              ┌──────────────┼──────────────┐
              │              │              │
              ▼              ▼              ▼
    ┌──────────────┐ ┌─────────────┐ ┌──────────────┐
    │  Designers   │ │  Suppliers  │ │   Admins     │
    ├──────────────┤ ├─────────────┤ ├──────────────┤
    │ PK: Id       │ │ PK: Id      │ │ (Future)     │
    │ FK: UserId   │ │ FK: UserId  │ └──────────────┘
    │ CompanyName  │ │ CompanyName │
    │ ContactNo    │ │ ContactNo   │
    │ Address      │ │ Address     │
    │ Website      │ │ MfgCapab.   │
    │ Rating       │ │ Rating      │
    │ CreatedAt    │ │ CreatedAt   │
    └──────────────┘ └─────────┬───┘
                               │
                               │ 1:N
                               ▼
                    ┌─────────────────────┐
                    │ SupplierCategories  │
                    ├─────────────────────┤
                    │ PK: Id              │
                    │ FK: SupplierId      │
                    │     Category (Enum) │
                    └─────────────────────┘

┌─────────────────────────────────────────────────────────────────────┐
│                    DATABASE: dressed_design                          │
└─────────────────────────────────────────────────────────────────────┘

┌──────────────────┐
│    Designers     │ (Replicated from auth DB)
├──────────────────┤
│ PK: Id           │◄────┐
│    CompanyName   │     │
│    ...           │     │ 1:N
└──────────────────┘     │
                         │
                         │
┌──────────────────────┐ │
│      Designs         │─┘
├──────────────────────┤
│ PK: Id               │
│ FK: DesignerId       │
│     Title            │
│     Description      │
│     Category         │
│     FileUrls (JSON)  │
│     Status           │
│     CreatedAt        │
│     Deadline         │
│     Quantity         │
│     Specifications   │
└──────────────────────┘

┌─────────────────────────────────────────────────────────────────────┐
│                    DATABASE: dressed_quote                           │
└─────────────────────────────────────────────────────────────────────┘

┌──────────────────┐
│    Suppliers     │ (Replicated from auth DB)
├──────────────────┤
│ PK: Id           │◄────┐
│    CompanyName   │     │
│    ...           │     │ 1:N
└──────────────────┘     │
                         │
                         │
┌──────────────────────┐ │
│       Quotes         │─┘
├──────────────────────┤
│ PK: Id               │
│ FK: DesignId         │ (from dressed_design.Designs)
│ FK: SupplierId       │
│     Price            │
│     Currency         │
│     DeliveryTime     │
│     QuoteText        │
│     Terms            │
│     Status           │
│     CreatedAt        │
│     UpdatedAt        │
└──────────────────────┘
```

### Status Enums

```
UserType:
  1 = Designer
  2 = Supplier
  3 = Admin

ClothingCategory:
  1 = Men
  2 = Women
  3 = Boy
  4 = Girl
  5 = Unisex

DesignStatus:
  1 = Draft
  2 = Published
  3 = QuotingOpen
  4 = Ordered
  5 = Completed

QuoteStatus:
  1 = Submitted
  2 = Accepted
  3 = Rejected
  4 = Expired
```

---

## Deployment Ports

| Service | Port | URL |
|---------|------|-----|
| Frontend | 3000 | http://localhost:3000 |
| API Gateway | 4000 | http://localhost:4000 |
| Auth Service | 5001 | http://localhost:5001 |
| Design Service | 5002 | http://localhost:5002 |
| Quote Service | 5003 | http://localhost:5003 |
| MySQL | 3306 | mysql://localhost:3306 |
| phpMyAdmin | 8080 | http://localhost:8080 |

---

## Security Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                     SECURITY LAYERS                          │
└─────────────────────────────────────────────────────────────┘

1. CLIENT LAYER
   ├─ HTTPS Encryption (Production)
   ├─ JWT Token Storage (localStorage)
   ├─ XSS Protection (React escaping)
   └─ CSRF Protection (Token validation)

2. API GATEWAY LAYER
   ├─ CORS Configuration
   ├─ Rate Limiting (Future)
   ├─ Token Validation
   └─ Request Sanitization

3. SERVICE LAYER
   ├─ Role-Based Authorization
   │  ├─ [Authorize(Roles = "Designer")]
   │  └─ [Authorize(Roles = "Supplier")]
   ├─ JWT Token Verification
   └─ Business Logic Validation

4. DATABASE LAYER
   ├─ Parameterized Queries (SQL Injection Prevention)
   ├─ Password Hashing (BCrypt)
   ├─ Database Isolation (Database-per-service)
   └─ Connection String Encryption

5. INFRASTRUCTURE LAYER
   ├─ Docker Network Isolation
   ├─ Container Security
   ├─ Volume Encryption (Production)
   └─ Environment Variable Management
```

---

## Performance Considerations

### Scalability Strategy

```
Current Architecture:
  Single instance of each service

Future Scalability:
  ┌─────────────────────────────┐
  │     Load Balancer (Nginx)   │
  └──────────────┬──────────────┘
                 │
     ┌───────────┼───────────┐
     │           │           │
     ▼           ▼           ▼
  Gateway1    Gateway2    Gateway3
     │           │           │
     ├───────────┼───────────┤
     │           │           │
     ▼           ▼           ▼
  ┌─────────────────────────────┐
  │   Service Mesh (Kubernetes) │
  │   - Auto-scaling            │
  │   - Load balancing          │
  │   - Health monitoring       │
  └─────────────────────────────┘
```

### Caching Strategy (Future)

```
Browser Cache
    ↓
CDN Cache (Static assets)
    ↓
API Gateway Cache
    ↓
Redis Cache (API responses)
    ↓
Database Query Cache
    ↓
Database
```

---

**Document Version**: 1.0  
**Last Updated**: December 23, 2025  
**Status**: Production Ready  
**Author**: Dressed™ Development Team
