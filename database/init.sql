-- Create databases
CREATE DATABASE IF NOT EXISTS dressed_auth;
CREATE DATABASE IF NOT EXISTS dressed_design;
CREATE DATABASE IF NOT EXISTS dressed_quote;
CREATE DATABASE IF NOT EXISTS dressed_order;
CREATE DATABASE IF NOT EXISTS dressed_communication;

-- Use auth database
USE dressed_auth;

-- Users table
CREATE TABLE IF NOT EXISTS Users (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Email VARCHAR(255) UNIQUE NOT NULL,
    PasswordHash VARCHAR(255) NOT NULL,
    FirstName VARCHAR(100) NOT NULL,
    LastName VARCHAR(100) NOT NULL,
    UserType INT NOT NULL,
    CreatedAt DATETIME NOT NULL,
    IsActive BOOLEAN DEFAULT TRUE,
    INDEX idx_email (Email),
    INDEX idx_usertype (UserType)
);

-- Designers table
CREATE TABLE IF NOT EXISTS Designers (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    UserId INT NOT NULL,
    CompanyName VARCHAR(255) NOT NULL,
    ContactNumber VARCHAR(50),
    Address TEXT,
    Website VARCHAR(255),
    Rating DECIMAL(3,2) DEFAULT 0,
    CreatedAt DATETIME NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    INDEX idx_userid (UserId)
);

-- Suppliers table
CREATE TABLE IF NOT EXISTS Suppliers (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    UserId INT NOT NULL,
    CompanyName VARCHAR(255) NOT NULL,
    ContactNumber VARCHAR(50),
    Address TEXT,
    ManufacturingCapabilities TEXT,
    Rating DECIMAL(3,2) DEFAULT 0,
    CreatedAt DATETIME NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    INDEX idx_userid (UserId)
);

-- Supplier Categories (for subscriptions)
CREATE TABLE IF NOT EXISTS SupplierCategories (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    SupplierId INT NOT NULL,
    Category INT NOT NULL,
    FOREIGN KEY (SupplierId) REFERENCES Suppliers(Id) ON DELETE CASCADE,
    UNIQUE KEY unique_supplier_category (SupplierId, Category),
    INDEX idx_category (Category)
);

-- Use design database
USE dressed_design;

-- Designs table
CREATE TABLE IF NOT EXISTS Designs (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    DesignerId INT NOT NULL,
    Title VARCHAR(255) NOT NULL,
    Description TEXT,
    Category INT NOT NULL,
    FileUrls JSON,
    Status INT NOT NULL,
    CreatedAt DATETIME NOT NULL,
    Deadline DATETIME,
    Quantity INT DEFAULT 1,
    Specifications TEXT,
    INDEX idx_designerid (DesignerId),
    INDEX idx_category (Category),
    INDEX idx_status (Status),
    INDEX idx_createdat (CreatedAt)
);

-- Copy Designers table structure for reference
CREATE TABLE IF NOT EXISTS Designers (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    UserId INT NOT NULL,
    CompanyName VARCHAR(255) NOT NULL,
    ContactNumber VARCHAR(50),
    Address TEXT,
    Website VARCHAR(255),
    Rating DECIMAL(3,2) DEFAULT 0,
    CreatedAt DATETIME NOT NULL,
    INDEX idx_userid (UserId)
);

-- Use quote database
USE dressed_quote;

-- Quotes table
CREATE TABLE IF NOT EXISTS Quotes (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    DesignId INT NOT NULL,
    SupplierId INT NOT NULL,
    Price DECIMAL(10,2) NOT NULL,
    Currency VARCHAR(10) DEFAULT 'USD',
    DeliveryTimeInDays INT NOT NULL,
    QuoteText TEXT,
    TermsAndConditions TEXT,
    Status INT NOT NULL,
    CreatedAt DATETIME NOT NULL,
    UpdatedAt DATETIME,
    INDEX idx_designid (DesignId),
    INDEX idx_supplierid (SupplierId),
    INDEX idx_status (Status),
    INDEX idx_createdat (CreatedAt)
);

-- Copy Suppliers table structure for reference
CREATE TABLE IF NOT EXISTS Suppliers (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    UserId INT NOT NULL,
    CompanyName VARCHAR(255) NOT NULL,
    ContactNumber VARCHAR(50),
    Address TEXT,
    ManufacturingCapabilities TEXT,
    Rating DECIMAL(3,2) DEFAULT 0,
    CreatedAt DATETIME NOT NULL,
    INDEX idx_userid (UserId)
);

-- Use order database
USE dressed_order;

-- Orders table
CREATE TABLE IF NOT EXISTS Orders (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    DesignId INT NOT NULL,
    DesignerId INT NOT NULL,
    SupplierId INT NOT NULL,
    QuoteId INT NOT NULL,
    OrderNumber VARCHAR(50) UNIQUE NOT NULL,
    TotalAmount DECIMAL(10,2) NOT NULL,
    Status INT NOT NULL,
    OrderDate DATETIME NOT NULL,
    ShippingDate DATETIME,
    DeliveryDate DATETIME,
    ShippingAddress TEXT,
    TrackingNumber VARCHAR(100),
    PaymentStatus INT NOT NULL,
    INDEX idx_ordernumber (OrderNumber),
    INDEX idx_designerid (DesignerId),
    INDEX idx_supplierid (SupplierId),
    INDEX idx_status (Status),
    INDEX idx_orderdate (OrderDate)
);

-- Use communication database
USE dressed_communication;

-- Messages table
CREATE TABLE IF NOT EXISTS Messages (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    SenderId INT NOT NULL,
    ReceiverId INT NOT NULL,
    DesignId INT,
    QuoteId INT,
    Content TEXT NOT NULL,
    SentAt DATETIME NOT NULL,
    IsRead BOOLEAN DEFAULT FALSE,
    INDEX idx_senderid (SenderId),
    INDEX idx_receiverid (ReceiverId),
    INDEX idx_designid (DesignId),
    INDEX idx_quoteid (QuoteId),
    INDEX idx_sentat (SentAt)
);

-- Insert sample data
USE dressed_auth;

-- Sample Designer User (password: designer123)
INSERT INTO Users (Email, PasswordHash, FirstName, LastName, UserType, CreatedAt, IsActive) 
VALUES ('designer@example.com', '$2a$11$KGZ3V5xqXF0YZR8OJ7iP0ewJ0Kd9YhZxrwQV2H6zW4P5zH7Q8P6Iq', 'John', 'Designer', 1, NOW(), TRUE);

SET @designer_user_id = LAST_INSERT_ID();

INSERT INTO Designers (UserId, CompanyName, ContactNumber, Address, Rating, CreatedAt)
VALUES (@designer_user_id, 'Fashion Forward Inc', '+1234567890', '123 Fashion Street, NY', 4.5, NOW());

-- Sample Supplier User (password: supplier123)
INSERT INTO Users (Email, PasswordHash, FirstName, LastName, UserType, CreatedAt, IsActive) 
VALUES ('supplier@example.com', '$2a$11$KGZ3V5xqXF0YZR8OJ7iP0ewJ0Kd9YhZxrwQV2H6zW4P5zH7Q8P6Iq', 'Jane', 'Supplier', 2, NOW(), TRUE);

SET @supplier_user_id = LAST_INSERT_ID();

INSERT INTO Suppliers (UserId, CompanyName, ContactNumber, Address, ManufacturingCapabilities, Rating, CreatedAt)
VALUES (@supplier_user_id, 'Quality Garments Ltd', '+0987654321', '456 Manufacturing Ave, CA', 'Men, Women, Unisex clothing manufacturing', 4.8, NOW());

-- Insert sample supplier categories
INSERT INTO SupplierCategories (SupplierId, Category) VALUES (1, 1); -- Men
INSERT INTO SupplierCategories (SupplierId, Category) VALUES (1, 2); -- Women
INSERT INTO SupplierCategories (SupplierId, Category) VALUES (1, 5); -- Unisex

USE dressed_design;

-- Copy designer info
INSERT INTO Designers (Id, UserId, CompanyName, ContactNumber, Address, Website, Rating, CreatedAt)
VALUES (1, 1, 'Fashion Forward Inc', '+1234567890', '123 Fashion Street, NY', '', 4.5, NOW());

-- Sample Design
INSERT INTO Designs (DesignerId, Title, Description, Category, FileUrls, Status, CreatedAt, Quantity, Specifications)
VALUES (1, 'Summer Collection - Casual Dress', 'Beautiful summer dress with floral patterns', 2, '["https://example.com/design1.pdf", "https://example.com/design1-img.jpg"]', 2, NOW(), 100, 'Cotton fabric, Available in sizes S, M, L, XL');

USE dressed_quote;

-- Copy supplier info
INSERT INTO Suppliers (Id, UserId, CompanyName, ContactNumber, Address, ManufacturingCapabilities, Rating, CreatedAt)
VALUES (1, 2, 'Quality Garments Ltd', '+0987654321', '456 Manufacturing Ave, CA', 'Men, Women, Unisex clothing manufacturing', 4.8, NOW());

-- Sample Quote
INSERT INTO Quotes (DesignId, SupplierId, Price, DeliveryTimeInDays, QuoteText, TermsAndConditions, Status, CreatedAt)
VALUES (1, 1, 1500.00, 30, 'We can manufacture this design with high quality cotton fabric. Price includes materials and manufacturing.', 'Payment: 50% advance, 50% on delivery. Delivery time may vary based on fabric availability.', 1, NOW());

-- Use communication database
USE dressed_communication;

-- Messages table
CREATE TABLE IF NOT EXISTS Messages (
    MessageId INT AUTO_INCREMENT PRIMARY KEY,
    SenderId INT NOT NULL,
    ReceiverId INT NOT NULL,
    Content TEXT NOT NULL,
    DesignId INT,
    QuoteId INT,
    IsRead BOOLEAN DEFAULT FALSE,
    CreatedAt DATETIME NOT NULL,
    INDEX idx_sender (SenderId),
    INDEX idx_receiver (ReceiverId),
    INDEX idx_design (DesignId),
    INDEX idx_quote (QuoteId),
    INDEX idx_created (CreatedAt)
);

-- Create payment database
CREATE DATABASE IF NOT EXISTS dressed_payment;
USE dressed_payment;

-- Payments table
CREATE TABLE IF NOT EXISTS Payments (
    PaymentId INT AUTO_INCREMENT PRIMARY KEY,
    OrderId INT NOT NULL,
    UserId INT NOT NULL,
    Amount DECIMAL(10,2) NOT NULL,
    PlatformFee DECIMAL(10,2) NOT NULL,
    Status VARCHAR(50) NOT NULL,
    PaymentMethod VARCHAR(50) NOT NULL,
    TransactionId VARCHAR(100),
    CreatedAt DATETIME NOT NULL,
    INDEX idx_order (OrderId),
    INDEX idx_user (UserId),
    INDEX idx_status (Status),
    INDEX idx_created (CreatedAt)
);

-- Transactions table
CREATE TABLE IF NOT EXISTS Transactions (
    TransactionId INT AUTO_INCREMENT PRIMARY KEY,
    PaymentId INT NOT NULL,
    Type VARCHAR(50) NOT NULL,
    Amount DECIMAL(10,2) NOT NULL,
    Status VARCHAR(50) NOT NULL,
    CreatedAt DATETIME NOT NULL,
    FOREIGN KEY (PaymentId) REFERENCES Payments(PaymentId) ON DELETE CASCADE,
    INDEX idx_payment (PaymentId),
    INDEX idx_type (Type),
    INDEX idx_created (CreatedAt)
);

COMMIT;
