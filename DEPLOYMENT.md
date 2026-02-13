# GitHub Actions CI/CD Deployment Guide

This guide explains how to set up GitHub Secrets for the CI/CD workflow.

## Setup Instructions

### 1. Create Azure App Services

Before setting up GitHub secrets, create two Azure App Services:

#### Backend App Service
```bash
az appservice plan create \
  --name myAppServicePlan \
  --resource-group myResourceGroup \
  --sku B1 --is-linux

az webapp create \
  --resource-group myResourceGroup \
  --plan myAppServicePlan \
  --name azure-webapp-backend \
  --runtime "DOTNETCORE|10.0"
```

#### Frontend App Service (if deploying separately)
```bash
az webapp create \
  --resource-group myResourceGroup \
  --plan myAppServicePlan \
  --name azure-webapp-frontend \
  --runtime "node|24-lts"
```

### 2. Download Publish Profiles

#### For Backend
1. Go to Azure Portal → App Services → your-backend-app
2. Click "Download Publish Profile" button
3. Save as `backend-publish-profile.xml`

#### For Frontend (if separate deployment)
1. Go to Azure Portal → App Services → your-frontend-app
2. Click "Download Publish Profile" button
3. Save as `frontend-publish-profile.xml`

### 3. Add GitHub Secrets

1. Go to your GitHub repository
2. Settings → Secrets and variables → Actions → New repository secret

Add these secrets:

#### Secret 1: Backend Publish Profile
- **Name:** `AZURE_BACKEND_PUBLISH_PROFILE`
- **Value:** Paste the entire contents of your backend publish profile XML

#### Secret 2: Backend App Name
- **Name:** `AZURE_BACKEND_APP_NAME`
- **Value:** `your-backend-app-name` (e.g., `azure-webapp-backend`)

#### Secret 3: Frontend Publish Profile (optional)
- **Name:** `AZURE_FRONTEND_PUBLISH_PROFILE`
- **Value:** Paste the entire contents of your frontend publish profile XML

#### Secret 4: Frontend App Name (optional)
- **Name:** `AZURE_FRONTEND_APP_NAME`
- **Value:** `your-frontend-app-name` (e.g., `azure-webapp-frontend`)

### 4. Recommended: Add CORS Configuration

Update your backend's `Program.cs` to allow requests from your Azure frontend:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAzureFrontend", policy =>
    {
        policy.WithOrigins(
            "https://your-frontend-app.azurewebsites.net",
            "http://localhost:4200", // Local development
            "https://localhost:4200"
        )
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
});

// In Configure section:
app.UseCors("AllowAzureFrontend");
```

## Workflow Overview

The GitHub Actions workflow performs the following steps:

### On Every Push to Main:

1. **Checkout Code** - Clones the repository
2. **Setup .NET 10** - Installs .NET SDK
3. **Setup Node.js** - Installs Node.js runtime
4. **Frontend Build**
   - Installs npm dependencies
   - Builds Angular with production settings
   - Output goes to `frontend/dist/`
5. **Copy Frontend to Backend**
   - Copies built frontend to `backend/wwwroot/`
   - Allows backend to serve frontend files
6. **Backend Build**
   - Restores NuGet packages
   - Builds .NET project in Release mode
   - Publishes to `backend/publish/`
7. **Deploy Backend**
   - Deploys to Azure App Service using publish profile
   - Includes frontend static files in wwwroot
8. **Deploy Frontend (Optional)**
   - Alternative: Deploy frontend separately if using dedicated app service

## Deployment Architecture

### Option 1: Combined Deployment (Recommended)
```
GitHub → Build Both → Deploy Backend to Azure App Service
         (Frontend copied to wwwroot/)
         
Backend serves both API and Frontend (SPA)
```

### Option 2: Separate Deployment
```
GitHub → Build Backend → Deploy to Backend App Service
      → Build Frontend → Deploy to Frontend App Service (storage/CDN)
```

## Monitoring Deployments

1. **GitHub Actions Status**
   - Go to Actions tab in your repository
   - See real-time build/deploy logs

2. **Azure Portal**
   - Go to App Services → Activity Log
   - See deployment history

3. **Check Deployment**
   ```bash
   # Backend is running at:
   https://your-backend-app.azurewebsites.net
   
   # API endpoint:
   https://your-backend-app.azurewebsites.net/api/products
   ```

## Troubleshooting

### Deployment Fails with Publish Profile Error
- Verify publish profile XML is complete (starts with `<?xml` and ends with `</publishProfile>`)
- Re-download from Azure Portal if corrupted
- Check secret name matches exactly: `AZURE_BACKEND_PUBLISH_PROFILE`

### Frontend Not Loading
- Check frontend is copied to `wwwroot/` in publish job
- Verify index.html exists in wwwroot
- Check Application Insights logs in Azure Portal

### API Calls Return 404
- Verify backend is using correct routing
- Check CORS configuration allows frontend origin
- Verify API endpoints are correct: `/api/products`

### Build Fails with Timeout
- Increase Azure App Service plan (B1 is minimum recommended)
- Consider using deployment slots for zero-downtime deploys

## Advanced Configuration

### Using Deployment Slots

Modify `.github/workflows/deploy.yml`:

```yaml
- name: Deploy to staging slot
  uses: azure/webapps-deploy@v2
  with:
    app-name: ${{ secrets.AZURE_BACKEND_APP_NAME }}
    publish-profile: ${{ secrets.AZURE_BACKEND_PUBLISH_PROFILE }}
    package: ./backend/publish/
    slot-name: staging

- name: Swap slots to production
  run: |
    az webapp deployment slot swap \
      --name ${{ secrets.AZURE_BACKEND_APP_NAME }} \
      --resource-group ${{ secrets.AZURE_RESOURCE_GROUP }} \
      --slot staging
```

### Adding Environment-Specific Configuration

Create `appsettings.Production.json`:

```json
{
  "FrontendUrl": "https://your-frontend-app.azurewebsites.net",
  "DatabaseConnection": "Server=your-db-server.database.windows.net;...",
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

Update `Program.cs`:

```csharp
var builder = WebApplication.CreateBuilder(args);

// Load environment-specific configuration
builder.Configuration.AddJsonFile("appsettings.json", true, true);
builder.Configuration.AddJsonFile(
    $"appsettings.{builder.Environment.EnvironmentName}.json", 
    true, 
    true
);
```

### Conditional Deployment

Only deploy on main branch with version tags:

```yaml
- name: Check Version Tag
  if: startsWith(github.ref, 'refs/tags/v')
  run: echo "Deploying version ${{ github.ref }}"
```

## Security Best Practices

✅ **Doing Right:**
- Publish profiles stored as encrypted secrets
- Different secrets for staging/production
- CI/CD only on main branch
- Deployment alerts enabled

❌ **Avoid:**
- Never commit publish profiles
- Never hardcode app names
- Avoid exposing secrets in logs
- Don't deploy on every branch

## Additional Resources

- [Azure WebApps Deploy GitHub Action](https://github.com/Azure/webapps-deploy)
- [GitHub Secrets Documentation](https://docs.github.com/en/actions/security-guides/encrypted-secrets)
- [Azure App Service Documentation](https://learn.microsoft.com/en-us/azure/app-service/)
- [GitHub Actions Documentation](https://docs.github.com/en/actions)

## Next Steps

1. ✅ Create Azure App Services
2. ✅ Download publish profiles
3. ✅ Add GitHub secrets
4. ✅ Update CORS in Program.cs
5. ✅ Push to main branch
6. ✅ Monitor Actions tab for deployment

---

For questions or issues, check the logs in the GitHub Actions tab or Azure Portal activity logs.
