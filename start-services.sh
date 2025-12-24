#!/bin/bash

# Navigate to the correct directory
cd "/Users/shehansalitha/Documents/CreditSource/DressedCore Backend"

echo "🚀 Starting Dressed™ Platform Services..."
echo "=========================================="
echo ""

# Step 1: Build new services
echo "📦 Building Communication and Payment services..."
docker-compose build communication-service payment-service

if [ $? -eq 0 ]; then
    echo "✅ Build successful!"
else
    echo "❌ Build failed. Please check the errors above."
    exit 1
fi

echo ""
echo "🔄 Starting all services..."
docker-compose up -d

echo ""
echo "⏳ Waiting for services to be ready..."
sleep 10

echo ""
echo "📊 Service Status:"
docker-compose ps

echo ""
echo "=========================================="
echo "🎉 All services should now be running!"
echo ""
echo "Access points:"
echo "  Frontend:              http://localhost:3000"
echo "  API Gateway:           http://localhost:4000"
echo "  Auth Service:          http://localhost:5001/swagger"
echo "  Design Service:        http://localhost:5002/swagger"
echo "  Quote Service:         http://localhost:5003/swagger"
echo "  Communication Service: http://localhost:5004/swagger"
echo "  Payment Service:       http://localhost:5005/swagger"
echo "  phpMyAdmin:            http://localhost:8080"
echo ""
echo "To view logs:"
echo "  docker-compose logs -f"
echo ""
echo "To stop all services:"
echo "  docker-compose down"
echo ""
