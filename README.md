# Angular + .NET API POC

A simple Proof of Concept demonstrating an Angular frontend consuming a .NET backend API with mock data.

## Project Structure

```
├── backend/          # ASP.NET Core Web API (.NET 10)
│   ├── Controllers/  # API endpoints
│   ├── Models/       # Data models
│   └── Program.cs    # Application configuration
│
└── frontend/         # Angular application
    ├── src/
    │   ├── app/
    │   │   ├── components/  # Angular components
    │   │   ├── services/    # Angular services
    │   │   └── app.routes.ts
    │   └── main.ts
    └── package.json
```

## Prerequisites

- .NET 10 SDK
- Node.js (v24+) and npm (v11+)
- A modern web browser

## Quick Start

### 1. Start the .NET Backend

```bash
cd backend
dotnet run
```

The backend will be available at: `https://localhost:7239`

**API Endpoints:**
- `GET /api/products` - Get all products
- `GET /api/products/{id}` - Get a specific product

### 2. Start the Angular Frontend (in a new terminal)

```bash
cd frontend
npm install  # First time only
npm start
```

The frontend will be available at: `http://localhost:4200`

## CI/CD & Deployment

This project includes GitHub Actions workflows for automated building and deployment to Azure App Service.

### Deployment Options

**Option 1: Combined Deployment** (Recommended)
- Single Azure App Service
- Backend serves both API and frontend static files
- Lower cost and simpler setup
- Workflow: `.github/workflows/deploy.yml`

**Option 2: Separate Deployment**
- Two Azure App Services (backend API + frontend)
- Independent scaling and deployment
- Better for larger applications
- Workflow: `.github/workflows/deploy-separate.yml`

### Setup Instructions

See [.github/WORKFLOWS_README.md](.github/WORKFLOWS_README.md) for detailed setup instructions including:
- Creating Azure App Services
- Downloading publish profiles
- Adding GitHub Secrets
- Configuring CORS for Azure
- Monitoring deployments

### Quick Deployment Setup

1. Create Azure App Services
2. Download publish profiles from Azure Portal
3. Add GitHub Secrets (publish profiles + app names)
4. Push to `main` branch
5. Monitor in GitHub Actions tab

For detailed steps, see [DEPLOYMENT.md](DEPLOYMENT.md)

## Features

### Backend (.NET)
- ASP.NET Core Web API
- RESTful API endpoints
- CORS configured for local development
- Mock data (no database required)
- 5 sample products with pricing and stock information

### Frontend (Angular)
- Standalone components
- HttpClient for API communication
- Responsive product grid layout
- Error handling and loading states
- Service-based architecture

## Mock Data

The backend provides 5 sample products:
1. Laptop - $999.99 (15 in stock)
2. Monitor - $449.99 (32 in stock)
3. Keyboard - $129.99 (50 in stock)
4. Mouse - $79.99 (45 in stock)
5. USB-C Hub - $49.99 (28 in stock)

## Architecture

### Data Flow
1. Angular component loads and initializes
2. Component calls ProductService on `ngOnInit`
3. ProductService makes HTTP GET request to backend
4. .NET API returns mock product data
5. Component displays products in a responsive grid

### CORS Configuration
The backend is configured to accept requests from:
- `http://localhost:4200`
- `https://localhost:4200`

## Troubleshooting

### Backend won't start
- Ensure .NET 10 is installed: `dotnet --version`
- Check port 7239 is available
- For HTTPS certificate issues: `dotnet dev-certs https --trust`

### Frontend won't connect to backend
- Verify backend is running on `https://localhost:7239`
- Check browser console for CORS errors
- Ensure firewall allows the connection

### npm install fails
- Delete `node_modules` and `package-lock.json`
- Run `npm cache clean --force`
- Try `npm install` again

## Future Enhancements

- Add database persistence (SQL Server/PostgreSQL)
- Implement authentication and authorization
- Add product creation, update, delete endpoints
- Implement pagination and filtering
- Add unit and integration tests
- Add product image support
- Implement shopping cart functionality
- Add product reviews and ratings

## Technology Stack

- **Backend:** ASP.NET Core 10, C#
- **Frontend:** Angular 19, TypeScript
- **Styling:** CSS
- **Communication:** HTTP/REST

## License

This is a sample POC for educational purposes.