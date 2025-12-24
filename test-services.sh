#!/bin/bash

# Test script for Communication and Payment services
cd "/Users/shehansalitha/Documents/CreditSource/DressedCore Backend"

echo "🧪 Testing Dressed™ Platform Services"
echo "======================================"
echo ""

# Colors
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Step 1: Login and get token
echo "${YELLOW}1. Logging in as designer...${NC}"
LOGIN_RESPONSE=$(curl -s -X POST http://localhost:4000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "designer@fashionforward.com",
    "password": "Designer@123"
  }')

TOKEN=$(echo $LOGIN_RESPONSE | grep -o '"token":"[^"]*' | cut -d'"' -f4)

if [ -z "$TOKEN" ]; then
    echo "${RED}❌ Login failed. Creating test user...${NC}"
    # Register designer
    curl -s -X POST http://localhost:4000/api/auth/register \
      -H "Content-Type: application/json" \
      -d '{
        "email": "designer@fashionforward.com",
        "password": "Designer@123",
        "firstName": "John",
        "lastName": "Designer",
        "userType": 1
      }' > /dev/null
    
    # Try login again
    LOGIN_RESPONSE=$(curl -s -X POST http://localhost:4000/api/auth/login \
      -H "Content-Type: application/json" \
      -d '{
        "email": "designer@fashionforward.com",
        "password": "Designer@123"
      }')
    TOKEN=$(echo $LOGIN_RESPONSE | grep -o '"token":"[^"]*' | cut -d'"' -f4)
fi

if [ ! -z "$TOKEN" ]; then
    echo "${GREEN}✅ Login successful!${NC}"
else
    echo "${RED}❌ Could not get authentication token${NC}"
    exit 1
fi

echo ""

# Step 2: Test Payment Service
echo "${YELLOW}2. Testing Payment Service...${NC}"

# Calculate platform fee
echo "  - Calculating platform fee for $1500..."
FEE_RESPONSE=$(curl -s -X POST http://localhost:4000/api/payments/calculate-fee \
  -H "Content-Type: application/json" \
  -d '{"amount": 1500.00}')

if echo $FEE_RESPONSE | grep -q "platformFee"; then
    echo "${GREEN}  ✅ Platform fee calculation works!${NC}"
    echo "  Response: $FEE_RESPONSE"
else
    echo "${RED}  ❌ Platform fee calculation failed${NC}"
fi

echo ""

# Process payment
echo "  - Processing test payment..."
PAYMENT_RESPONSE=$(curl -s -X POST http://localhost:4000/api/payments/process \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "orderId": 1,
    "amount": 1500.00,
    "paymentMethod": "CreditCard"
  }')

if echo $PAYMENT_RESPONSE | grep -q "success"; then
    echo "${GREEN}  ✅ Payment processing works!${NC}"
    PAYMENT_ID=$(echo $PAYMENT_RESPONSE | grep -o '"paymentId":[0-9]*' | cut -d':' -f2)
    echo "  Payment ID: $PAYMENT_ID"
else
    echo "${RED}  ❌ Payment processing failed${NC}"
    echo "  Response: $PAYMENT_RESPONSE"
fi

echo ""

# Get payment history
echo "  - Getting payment history..."
HISTORY_RESPONSE=$(curl -s -X GET http://localhost:4000/api/payments/user \
  -H "Authorization: Bearer $TOKEN")

if echo $HISTORY_RESPONSE | grep -q "paymentId"; then
    echo "${GREEN}  ✅ Payment history retrieval works!${NC}"
else
    echo "${RED}  ❌ Payment history retrieval failed${NC}"
fi

echo ""

# Step 3: Test Communication Service
echo "${YELLOW}3. Testing Communication Service...${NC}"

# Get unread message count
echo "  - Getting unread message count..."
UNREAD_RESPONSE=$(curl -s -X GET http://localhost:4000/api/messages/unread/count \
  -H "Authorization: Bearer $TOKEN")

if echo $UNREAD_RESPONSE | grep -q "unreadCount"; then
    echo "${GREEN}  ✅ Unread count works!${NC}"
    echo "  Response: $UNREAD_RESPONSE"
else
    echo "${RED}  ❌ Unread count failed${NC}"
fi

echo ""

# Send a message
echo "  - Sending test message..."
MESSAGE_RESPONSE=$(curl -s -X POST http://localhost:4000/api/messages \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "receiverId": 2,
    "content": "Hello! I have a question about the quote.",
    "designId": 1
  }')

if echo $MESSAGE_RESPONSE | grep -q "messageId"; then
    echo "${GREEN}  ✅ Message sending works!${NC}"
    MESSAGE_ID=$(echo $MESSAGE_RESPONSE | grep -o '"messageId":[0-9]*' | cut -d':' -f2)
    echo "  Message ID: $MESSAGE_ID"
else
    echo "${RED}  ❌ Message sending failed${NC}"
    echo "  Response: $MESSAGE_RESPONSE"
fi

echo ""

# Get user messages
echo "  - Getting user messages..."
MESSAGES_RESPONSE=$(curl -s -X GET http://localhost:4000/api/messages \
  -H "Authorization: Bearer $TOKEN")

if echo $MESSAGES_RESPONSE | grep -q "\["; then
    echo "${GREEN}  ✅ Message retrieval works!${NC}"
else
    echo "${RED}  ❌ Message retrieval failed${NC}"
fi

echo ""
echo "======================================"
echo "${GREEN}🎉 Testing Complete!${NC}"
echo ""
echo "Service URLs:"
echo "  Communication Service: http://localhost:5004/swagger"
echo "  Payment Service:       http://localhost:5005/swagger"
echo ""
