-- Create the database if it doesn't exist
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'MaskingDemo')
BEGIN
    CREATE DATABASE MaskingDemo;
END;

-- Use the created database
USE MaskingDemo;

-- Drop existing tables to ensure the script is idempotent
DROP TABLE IF EXISTS Addresses;
DROP TABLE IF EXISTS Accounts;
DROP TABLE IF EXISTS Users;

-- Create Users Table
CREATE TABLE Users (
    Id INT IDENTITY PRIMARY KEY,
    Name NVARCHAR(100),
    SSN NVARCHAR(50),
    Email NVARCHAR(100),
    Phone NVARCHAR(20)
);

-- Create Accounts Table
CREATE TABLE Accounts (
    Id INT IDENTITY PRIMARY KEY,
    AccountNumber NVARCHAR(20),
    Balance DECIMAL(18, 2),
    CreditCard NVARCHAR(50)
);

-- Create Addresses Table
CREATE TABLE Addresses (
    Id INT IDENTITY PRIMARY KEY,
    UserId INT,
    StreetAddress NVARCHAR(200),
    City NVARCHAR(100),
    State NVARCHAR(50),
    PostalCode NVARCHAR(20),
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);

-- Seed Users Table
INSERT INTO Users (Name, SSN, Email, Phone) VALUES
('John Doe', '123-45-6789', 'john.doe@example.com', '123-456-7890'),
('Jane Smith', '987-65-4321', 'jane.smith@example.com', '234-567-8901'),
('Alice Johnson', '456-78-9123', 'alice.johnson@example.com', '345-678-9012'),
('Bob Brown', '654-32-1987', 'bob.brown@example.com', '456-789-0123'),
('Carol Davis', '321-54-8765', 'carol.davis@example.com', '567-890-1234');

-- Seed Accounts Table
INSERT INTO Accounts (AccountNumber, Balance, CreditCard) VALUES
('1111222233334444', 1500.00, '4111111111111111'),
('4444333322221111', 2500.00, '4222222222222222'),
('5555666677778888', 3500.00, '4333333333333333'),
('8888777766665555', 4500.00, '4444444444444444'),
('9999000011112222', 5500.00, '4555555555555555');

-- Seed Addresses Table
INSERT INTO Addresses (UserId, StreetAddress, City, State, PostalCode) VALUES
(1, '123 Main St', 'New York', 'NY', '10001'),
(2, '456 Elm St', 'Los Angeles', 'CA', '90001'),
(3, '789 Oak St', 'Chicago', 'IL', '60601'),
(4, '101 Pine St', 'Houston', 'TX', '77001'),
(5, '202 Maple St', 'Phoenix', 'AZ', '85001');
