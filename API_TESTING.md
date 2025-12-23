# API Testing Guide - Dressed™

## Testing with curl

### 1. Register a Designer

```bash
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "newdesigner@example.com",
    "password": "password123",
    "firstName": "John",
    "lastName": "Doe",
    "userType": "Designer",
    "companyName": "Fashion Forward Co",
    "contactNumber": "+1234567890",
    "address": "123 Fashion Street, New York, NY 10001"
  }'
```

Expected Response (200 OK):
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "email": "newdesigner@example.com",
  "firstName": "John",
  "lastName": "Doe",
  "userType": "Designer",
  "userId": 2
}
```

### 2. Register a Supplier

```bash
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "newsupplier@example.com",
    "password": "password123",
    "firstName": "Jane",
    "lastName": "Smith",
    "userType": "Supplier",
    "companyName": "Quality Garments Ltd",
    "contactNumber": "+0987654321",
    "address": "456 Manufacturing Ave, Los Angeles, CA 90001"
  }'
```

### 3. Login as Designer

```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "designer@example.com",
    "password": "designer123"
  }'
```

**Save the token from response!**

### 4. Create a Design (Designer)

```bash
# Replace YOUR_TOKEN_HERE with actual token
curl -X POST http://localhost:5000/api/designs \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN_HERE" \
  -d '{
    "title": "Summer Collection 2025 - Floral Dress",
    "description": "Beautiful summer dress with vibrant floral patterns, perfect for beach and casual wear",
    "category": "Women",
    "fileUrls": [
      "https://example.com/designs/summer-dress-front.jpg",
      "https://example.com/designs/summer-dress-back.jpg",
      "https://example.com/designs/specifications.pdf"
    ],
    "deadline": "2025-03-01T00:00:00Z",
    "quantity": 500,
    "specifications": "Material: 100% Cotton\nSizes: S, M, L, XL\nColors: Blue, Pink, Yellow\nPackaging: Individual polybags"
  }'
```

### 5. View All Designs (Public)

```bash
curl http://localhost:5000/api/designs
```

### 6. View Designs by Category

```bash
# Available categories: Men, Women, Boy, Girl, Unisex
curl "http://localhost:5000/api/designs?category=Women"
```

### 7. View Specific Design

```bash
curl http://localhost:5000/api/designs/1
```

### 8. Login as Supplier

```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "supplier@example.com",
    "password": "supplier123"
  }'
```

**Save the supplier token!**

### 9. Submit a Quote (Supplier)

```bash
# Replace SUPPLIER_TOKEN_HERE with actual token
curl -X POST http://localhost:5000/api/quotes \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer SUPPLIER_TOKEN_HERE" \
  -d '{
    "designId": 1,
    "price": 2500.00,
    "deliveryTimeInDays": 45,
    "quoteText": "We can manufacture this design with premium quality materials. Our facility specializes in summer wear and we have extensive experience with floral patterns. Price includes all materials, labor, and quality control.",
    "termsAndConditions": "Payment Terms: 50% advance, 50% on delivery\nDelivery: FOB our facility\nQuality: ISO 9001 certified\nWarranty: 30 days for manufacturing defects"
  }'
```

### 10. View Quotes for a Design (Designer)

```bash
# Replace DESIGNER_TOKEN_HERE with actual token
curl http://localhost:5000/api/quotes/design/1 \
  -H "Authorization: Bearer DESIGNER_TOKEN_HERE"
```

### 11. View My Quotes (Supplier)

```bash
# Replace SUPPLIER_TOKEN_HERE with actual token
curl http://localhost:5000/api/quotes/supplier/1 \
  -H "Authorization: Bearer SUPPLIER_TOKEN_HERE"
```

### 12. View My Designs (Designer)

```bash
# Replace DESIGNER_TOKEN_HERE with actual token
curl http://localhost:5000/api/designs/designer/1 \
  -H "Authorization: Bearer DESIGNER_TOKEN_HERE"
```

### 13. Update Design Status

```bash
# Replace DESIGNER_TOKEN_HERE with actual token
# Status options: Draft, Published, QuotingOpen, QuotingClosed, Ordered, Completed, Cancelled
curl -X PATCH http://localhost:5000/api/designs/1/status \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer DESIGNER_TOKEN_HERE" \
  -d '"QuotingOpen"'
```

### 14. Update Quote Status

```bash
# Replace TOKEN_HERE with actual token
# Status options: Submitted, UnderNegotiation, Accepted, Rejected, Expired
curl -X PATCH http://localhost:5000/api/quotes/1/status \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer TOKEN_HERE" \
  -d '"Accepted"'
```

### 15. Health Checks

```bash
# Auth Service
curl http://localhost:5001/api/auth/health

# Design Service
curl http://localhost:5002/api/designs/health

# Quote Service
curl http://localhost:5003/api/quotes/health
```

---

## Testing with HTTPie (Alternative)

If you prefer HTTPie (https://httpie.io/):

### Register
```bash
http POST localhost:5000/api/auth/register \
  email=designer@test.com \
  password=test123 \
  firstName=John \
  lastName=Doe \
  userType=Designer \
  companyName="Test Company" \
  contactNumber="+1234567890" \
  address="123 Test St"
```

### Login
```bash
http POST localhost:5000/api/auth/login \
  email=designer@example.com \
  password=designer123
```

### Create Design (with token)
```bash
http POST localhost:5000/api/designs \
  Authorization:"Bearer YOUR_TOKEN" \
  title="New Design" \
  description="Description" \
  category=Women \
  fileUrls:='["url1.jpg"]' \
  quantity:=100 \
  specifications="Cotton fabric"
```

---

## Testing with Postman

### Setup

1. **Create Environment**:
   - Name: Dressed Local
   - Variables:
     - `base_url`: http://localhost:5000
     - `designer_token`: (leave empty, will be set after login)
     - `supplier_token`: (leave empty, will be set after login)

2. **Create Collection**: "Dressed API Tests"

### Test Scripts

Add this to Login requests to auto-save token:

```javascript
// Tests tab in Postman
pm.test("Status code is 200", function () {
    pm.response.to.have.status(200);
});

pm.test("Token is present", function () {
    var jsonData = pm.response.json();
    pm.expect(jsonData.token).to.exist;
    
    // Save token to environment
    if (jsonData.userType === "Designer") {
        pm.environment.set("designer_token", jsonData.token);
    } else if (jsonData.userType === "Supplier") {
        pm.environment.set("supplier_token", jsonData.token);
    }
});
```

For authenticated requests, use:
```
Authorization: Bearer {{designer_token}}
```

---

## Common Test Scenarios

### Scenario 1: Complete Designer Flow

```bash
# 1. Register
TOKEN=$(curl -s -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"email":"flow@test.com","password":"test123","firstName":"Test","lastName":"User","userType":"Designer","companyName":"Test Co","contactNumber":"+123","address":"123 St"}' \
  | jq -r '.token')

# 2. Create Design
DESIGN_ID=$(curl -s -X POST http://localhost:5000/api/designs \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{"title":"Test Design","description":"Test","category":"Women","fileUrls":["test.jpg"],"quantity":100,"specifications":"Test spec"}' \
  | jq -r '.id')

# 3. View My Designs
curl http://localhost:5000/api/designs/designer/1 \
  -H "Authorization: Bearer $TOKEN"

echo "Design ID: $DESIGN_ID"
```

### Scenario 2: Complete Supplier Flow

```bash
# 1. Login as Supplier
TOKEN=$(curl -s -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"supplier@example.com","password":"supplier123"}' \
  | jq -r '.token')

# 2. View Available Designs
curl http://localhost:5000/api/designs

# 3. Submit Quote
curl -X POST http://localhost:5000/api/quotes \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{"designId":1,"price":1500,"deliveryTimeInDays":30,"quoteText":"Competitive quote","termsAndConditions":"Standard terms"}'

# 4. View My Quotes
curl http://localhost:5000/api/quotes/supplier/1 \
  -H "Authorization: Bearer $TOKEN"
```

### Scenario 3: Negotiation Flow

```bash
# Designer views quotes
curl http://localhost:5000/api/quotes/design/1 \
  -H "Authorization: Bearer $DESIGNER_TOKEN"

# Designer marks quote for negotiation
curl -X PATCH http://localhost:5000/api/quotes/1/status \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $DESIGNER_TOKEN" \
  -d '"UnderNegotiation"'

# Supplier updates quote status
curl -X PATCH http://localhost:5000/api/quotes/1/status \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $SUPPLIER_TOKEN" \
  -d '"Accepted"'
```

---

## Error Response Examples

### 400 Bad Request
```json
{
  "message": "Registration failed. Email may already exist."
}
```

### 401 Unauthorized
```json
{
  "message": "Invalid email or password."
}
```

### 404 Not Found
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
  "title": "Not Found",
  "status": 404
}
```

---

## Database Queries (phpMyAdmin)

Access: http://localhost:8080 (root/rootpassword)

### View All Users
```sql
SELECT * FROM dressed_auth.Users;
```

### View All Designs with Designer Info
```sql
SELECT d.*, des.CompanyName, des.Rating
FROM dressed_design.Designs d
LEFT JOIN dressed_design.Designers des ON d.DesignerId = des.Id
ORDER BY d.CreatedAt DESC;
```

### View All Quotes with Supplier Info
```sql
SELECT q.*, s.CompanyName, s.Rating
FROM dressed_quote.Quotes q
LEFT JOIN dressed_quote.Suppliers s ON q.SupplierId = s.Id
ORDER BY q.CreatedAt DESC;
```

### Get Quote Statistics
```sql
SELECT 
    Status,
    COUNT(*) as Count,
    AVG(Price) as AvgPrice,
    MIN(Price) as MinPrice,
    MAX(Price) as MaxPrice
FROM dressed_quote.Quotes
GROUP BY Status;
```

---

## Performance Testing

### Load Test with Apache Bench

```bash
# Test login endpoint
ab -n 1000 -c 10 -p login.json -T application/json \
  http://localhost:5000/api/auth/login

# login.json content:
# {"email":"designer@example.com","password":"designer123"}
```

### Load Test with wrk

```bash
# Install wrk: brew install wrk

# Test GET endpoint
wrk -t4 -c100 -d30s http://localhost:5000/api/designs

# Test POST with lua script
wrk -t4 -c100 -d30s -s post.lua http://localhost:5000/api/designs
```

---

## Monitoring

### Container Stats
```bash
docker stats
```

### Service Logs
```bash
# All services
docker-compose logs -f

# Specific service
docker-compose logs -f auth-service
docker-compose logs -f design-service
docker-compose logs -f quote-service
```

### Database Connections
```sql
SHOW PROCESSLIST;
```

---

## Troubleshooting

### Token Issues
If you get 401 Unauthorized, check:
1. Token is included in Authorization header
2. Token format: `Bearer YOUR_TOKEN`
3. Token hasn't expired (24 hour default)
4. User has correct role for endpoint

### Connection Issues
```bash
# Ping services
curl http://localhost:5001/api/auth/health
curl http://localhost:5002/api/designs/health
curl http://localhost:5003/api/quotes/health

# Check containers
docker-compose ps

# Restart if needed
docker-compose restart
```

---

**Happy Testing! 🧪**
