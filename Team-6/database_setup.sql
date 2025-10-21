-- PostgreSQL Database Setup Script for Team-6 Microservices
-- Single database with multiple schemas approach

-- ==============================================
-- 1. CREATE DATABASE
-- ==============================================

-- Create single database for all services
CREATE DATABASE atmccdb;

-- ==============================================
-- 2. CONNECT TO DATABASE AND CREATE SCHEMAS
-- ==============================================

\c atmccdb;

-- Create schemas for each service
CREATE SCHEMA IF NOT EXISTS auth;
CREATE SCHEMA IF NOT EXISTS conversation;
CREATE SCHEMA IF NOT EXISTS profile;
CREATE SCHEMA IF NOT EXISTS channel;

-- ==============================================
-- 3. AUTH SCHEMA TABLES
-- ==============================================

-- Users table
CREATE TABLE auth.users (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    full_name VARCHAR(100) NOT NULL,
    login VARCHAR(255) NOT NULL UNIQUE,
    passwords_hash TEXT NOT NULL,
    role VARCHAR(50) NOT NULL CHECK (role IN ('Administrator', 'Worker')),
    is_active BOOLEAN NOT NULL DEFAULT true,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- Tokens table (for JWT token management)
CREATE TABLE auth.tokens (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL REFERENCES auth.users(id) ON DELETE CASCADE,
    token_hash TEXT NOT NULL,
    expires_at TIMESTAMP WITH TIME ZONE NOT NULL,
    is_revoked BOOLEAN NOT NULL DEFAULT false,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- Indexes for auth schema
CREATE INDEX idx_users_login ON auth.users(login);
CREATE INDEX idx_users_role ON auth.users(role);
CREATE INDEX idx_users_active ON auth.users(is_active);
CREATE INDEX idx_tokens_user_id ON auth.tokens(user_id);
CREATE INDEX idx_tokens_expires_at ON auth.tokens(expires_at);

-- ==============================================
-- 4. CONVERSATION SCHEMA TABLES
-- ==============================================

-- Conversations table
CREATE TABLE conversation.conversations (
    conversation_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    channel VARCHAR(50) NOT NULL CHECK (channel IN ('Telegram', 'Vk', 'Email')),
    message TEXT NOT NULL,
    status VARCHAR(50) NOT NULL CHECK (status IN ('New', 'Distributed', 'InWork', 'Closed', 'AgentNotFound')),
    worker_id UUID,
    create_date TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    update_date TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    answer TEXT,
    prefix_number VARCHAR(20) DEFAULT 'ВХ-ОБР-',
    number BIGSERIAL,
    user_id BIGINT,
    channel_settings_id INTEGER NOT NULL
);

-- Indexes for conversation schema
CREATE INDEX idx_conversations_status ON conversation.conversations(status);
CREATE INDEX idx_conversations_worker_id ON conversation.conversations(worker_id);
CREATE INDEX idx_conversations_create_date ON conversation.conversations(create_date);
CREATE INDEX idx_conversations_channel ON conversation.conversations(channel);
CREATE INDEX idx_conversations_number ON conversation.conversations(number);
CREATE INDEX idx_conversations_channel_settings_id ON conversation.conversations(channel_settings_id);

-- ==============================================
-- 5. PROFILE SCHEMA TABLES
-- ==============================================

-- Users table
CREATE TABLE profile.users (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    login VARCHAR(255) NOT NULL UNIQUE,
    password_hash TEXT NOT NULL,
    is_active BOOLEAN NOT NULL DEFAULT true,
    edit_date TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- User profiles table
CREATE TABLE profile.user_profiles (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL REFERENCES profile.users(id) ON DELETE CASCADE,
    first_name VARCHAR(100),
    last_name VARCHAR(100),
    middle_name VARCHAR(100),
    edit_date TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- Clients table
CREATE TABLE profile.clients (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(255) NOT NULL,
    edit_date TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- Client channels table
CREATE TABLE profile.client_channels (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    client_id UUID NOT NULL REFERENCES profile.clients(id) ON DELETE CASCADE,
    external_channel_type VARCHAR(100) NOT NULL,
    external_id VARCHAR(255) NOT NULL,
    edit_date TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- Indexes for profile schema
CREATE INDEX idx_users_login_profile ON profile.users(login);
CREATE INDEX idx_user_profiles_user_id ON profile.user_profiles(user_id);
CREATE INDEX idx_client_channels_client_id ON profile.client_channels(client_id);
CREATE INDEX idx_client_channels_external_type ON profile.client_channels(external_channel_type);

-- ==============================================
-- 6. CHANNEL SCHEMA TABLES
-- ==============================================

-- Channels table
CREATE TABLE channel.channels (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    token TEXT NOT NULL,
    type VARCHAR(50) NOT NULL CHECK (type IN ('Telegram', 'Vk', 'Email'))
);

-- Indexes for channel schema
CREATE INDEX idx_channels_type ON channel.channels(type);
CREATE INDEX idx_channels_name ON channel.channels(name);

-- ==============================================
-- 7. SAMPLE DATA INSERTION
-- ==============================================

-- Insert sample admin user in auth schema
INSERT INTO auth.users (id, full_name, login, passwords_hash, role, is_active) VALUES 
('550e8400-e29b-41d4-a716-446655440000', 'Администратор', 'admin@company.com', '$2a$11$example_hash_here', 'Administrator', true);

-- Insert sample worker user
INSERT INTO auth.users (id, full_name, login, passwords_hash, role, is_active) VALUES 
('550e8400-e29b-41d4-a716-446655440001', 'Оператор Иванов', 'operator@company.com', '$2a$11$example_hash_here', 'Worker', true);

-- Insert sample channels
INSERT INTO channel.channels (name, token, type) VALUES 
('Основной Telegram', 'bot_token_here', 'Telegram'),
('VK Группа', 'vk_token_here', 'Vk'),
('Email Поддержка', 'email_config_here', 'Email');

-- Insert sample conversations
INSERT INTO conversation.conversations (conversation_id, channel, message, status, worker_id, user_id, channel_settings_id) VALUES 
('550e8400-e29b-41d4-a716-446655440010', 'Telegram', 'Привет, у меня проблема с заказом', 'New', NULL, 12345, 1),
('550e8400-e29b-41d4-a716-446655440011', 'Vk', 'Когда будет готов мой заказ?', 'InWork', '550e8400-e29b-41d4-a716-446655440001', 12346, 2),
('550e8400-e29b-41d4-a716-446655440012', 'Email', 'Спасибо за помощь!', 'Closed', '550e8400-e29b-41d4-a716-446655440001', 12347, 3);

-- ==============================================
-- 8. GRANTS AND PERMISSIONS
-- ==============================================

-- Create application user (adjust username/password as needed)
-- CREATE USER app_user WITH PASSWORD 'your_secure_password';

-- Grant permissions for all schemas
-- GRANT USAGE ON SCHEMA auth, conversation, profile, channel TO app_user;
-- GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA auth TO app_user;
-- GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA conversation TO app_user;
-- GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA profile TO app_user;
-- GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA channel TO app_user;
-- GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA auth TO app_user;
-- GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA conversation TO app_user;
-- GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA profile TO app_user;
-- GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA channel TO app_user;

-- ==============================================
-- 9. CONNECTION STRINGS FOR REFERENCE
-- ==============================================

/*
Connection strings for appsettings.json files:

Auth Service:
"ConnectionStrings": {
  "ApplicationDbContext": "Host=localhost;Database=atmccdb;Username=postgres;Password=admin;Port=5432"
}

Conversation Service:
"ConnectionStrings": {
  "Conversation": "Host=localhost;Database=atmccdb;Username=postgres;Password=admin;Port=5432"
}

Profile Service:
"ConnectionStrings": {
  "Profile": "Host=localhost;Database=atmccdb;Username=postgres;Password=admin;Port=5432"
}

Channel Settings:
"ConnectionStrings": {
  "ApplicationDbContext": "Host=localhost;Database=atmccdb;Username=postgres;Password=admin;Port=5432"
}
*/

-- ==============================================
-- SCRIPT COMPLETION
-- ==============================================

SELECT 'Database setup completed successfully!' as status;
SELECT 'Schemas created: auth, conversation, profile, channel' as schemas;
SELECT 'Database: atmccdb' as database_name;