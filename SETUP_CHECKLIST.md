# GitHub Actions CI/CD - Setup Checklist

Use this checklist to set up CI/CD for Azure App Service deployment.

## Phase 1: Pre-Setup

- [ ] Have Azure subscription access
- [ ] Have GitHub repository admin access
- [ ] Have Azure CLI or Portal access
- [ ] Read `.github/WORKFLOWS_README.md`
- [ ] Choose deployment option (Option A vs B)

## Phase 2: Azure Resources

### Option A: Combined Deployment (Single App Service)

- [ ] Create Azure Resource Group
  ```bash
  az group create --name my-rg --location eastus
  ```

- [ ] Create App Service Plan
  ```bash
  az appservice plan create \
    --name my-plan \
    --resource-group my-rg \
    --sku B1 --is-linux
  ```

- [ ] Create Backend App Service
  ```bash
  az webapp create \
    --name my-backend-app \
    --resource-group my-rg \
    --plan my-plan \
    --runtime "DOTNETCORE|10.0"
  ```

- [ ] Enable HTTPS only
  ```bash
  az webapp update \
    --name my-backend-app \
    --resource-group my-rg \
    --set httpsOnly=true
  ```

### Option B: Separate Deployment (Two App Services)

Do everything above PLUS:

- [ ] Create Frontend App Service
  ```bash
  az webapp create \
    --name my-frontend-app \
    --resource-group my-rg \
    --plan my-plan \
    --runtime "node|24-lts"
  ```

- [ ] Enable HTTPS only for Frontend
  ```bash
  az webapp update \
    --name my-frontend-app \
    --resource-group my-rg \
    --set httpsOnly=true
  ```

## Phase 3: Publish Profiles

### Backend Publish Profile

- [ ] Go to Azure Portal
- [ ] Navigate to App Services → Backend App
- [ ] Click "Download Publish Profile" button
- [ ] Save the XML file
- [ ] Verify file contains `<?xml` and `</publishProfile>`

### Frontend Publish Profile (Option B only)

- [ ] Go to Azure Portal
- [ ] Navigate to App Services → Frontend App
- [ ] Click "Download Publish Profile" button
- [ ] Save the XML file
- [ ] Verify file contains `<?xml` and `</publishProfile>`

## Phase 4: GitHub Secrets

### Navigate to Secrets

- [ ] Go to GitHub.com
- [ ] Open your repository
- [ ] Click Settings tab
- [ ] Click "Secrets and variables" → Actions
- [ ] Click "New repository secret"

### Add Secrets - Option A (Combined)

- [ ] Add Secret #1
  - **Name:** `AZURE_BACKEND_APP_NAME`
  - **Value:** `my-backend-app` (your actual app name)
  - Click "Add secret"

- [ ] Add Secret #2
  - **Name:** `AZURE_BACKEND_PUBLISH_PROFILE`
  - **Value:** (paste entire XML file content)
  - Click "Add secret"

### Add Secrets - Option B (Separate)

Do all Option A steps above, PLUS:

- [ ] Add Secret #3
  - **Name:** `AZURE_FRONTEND_APP_NAME`
  - **Value:** `my-frontend-app` (your actual app name)
  - Click "Add secret"

- [ ] Add Secret #4
  - **Name:** `AZURE_FRONTEND_PUBLISH_PROFILE`
  - **Value:** (paste entire XML file content)
  - Click "Add secret"

## Phase 5: Configuration

### For Option B Only - Update CORS

- [ ] Open `backend/appsettings.Production.json`
- [ ] Update `AzureFrontendUrl` with your frontend app URL:
  ```json
  "AzureFrontendUrl": "https://my-frontend-app.azurewebsites.net"
  ```
- [ ] Save the file

### For Both Options - Verify Program.cs

- [ ] Check `backend/Program.cs` has:
  - [ ] `app.UseStaticFiles();`
  - [ ] `MapFallbackToFile("index.html")`
  - [ ] CORS configuration with `AddCors`

## Phase 6: Verify Workflows

- [ ] Check `.github/workflows/deploy.yml` exists
- [ ] Verify YAML syntax: Open file and check indentation
- [ ] (Option B) Check `.github/workflows/deploy-separate.yml` exists

## Phase 7: Commit and Push

- [ ] Add all files to git:
  ```bash
  git add .
  ```

- [ ] Commit changes:
  ```bash
  git commit -m "Add GitHub Actions CI/CD workflows for Azure deployment"
  ```

- [ ] Push to main:
  ```bash
  git push origin main
  ```

- [ ] Verify nothing failed:
  ```bash
  # Check if push was successful
  git log --oneline | head -1
  ```

## Phase 8: Monitor First Deployment

- [ ] Go to GitHub.com → Your repo
- [ ] Click "Actions" tab
- [ ] See your workflow running
- [ ] Wait for it to complete (5-15 minutes)
- [ ] Check for green checkmark ✓

### If Build Fails

- [ ] Click the failed workflow
- [ ] Click "Build and Deploy" job
- [ ] Read the error message
- [ ] Check troubleshooting in `.github/WORKFLOWS_README.md`
- [ ] Fix the issue locally
- [ ] Commit and push again

## Phase 9: Verify Deployment

### Option A - Combined Deployment

- [ ] Open browser
- [ ] Go to: `https://my-backend-app.azurewebsites.net`
- [ ] Should see "Product Store" page
- [ ] Check console for any errors
- [ ] Test API endpoint:
  ```bash
  curl https://my-backend-app.azurewebsites.net/api/products
  ```

### Option B - Separate Deployment

- [ ] Open browser
- [ ] Frontend: Go to `https://my-frontend-app.azurewebsites.net`
- [ ] Should see "Product Store" page
- [ ] Backend API: 
  ```bash
  curl https://my-backend-app.azurewebsites.net/api/products
  ```
- [ ] Both should work and return product data

## Phase 10: Troubleshooting (if needed)

### Frontend Shows 404

- [ ] Check GitHub Actions build log
- [ ] Verify build succeeded with green checkmark
- [ ] For Option A: Check `backend/wwwroot/index.html` exists (via Azure Portal)
- [ ] For Option B: Check frontend app deployed correctly
- [ ] Clear browser cache and reload

### API Returns 404 or CORS Error

- [ ] Check `backend/Program.cs` has `MapControllers()`
- [ ] For Option A: Should work (same origin)
- [ ] For Option B: Verify `AzureFrontendUrl` in `appsettings.Production.json`
- [ ] Check GitHub Actions logs for any errors

### Publish Profile Error

- [ ] Download fresh profile from Azure Portal
- [ ] Ensure entire XML is pasted (multi-line)
- [ ] Check secret name matches exactly
- [ ] Verify no extra spaces or newlines
- [ ] Try again

### Build Timeout

- [ ] Increase App Service plan to B2 or higher
- [ ] Clear local npm cache: `npm cache clean --force`
- [ ] Try again

## Phase 11: Set Up Monitoring

- [ ] Enable Application Insights in Azure Portal (optional)
- [ ] Set up email alerts for failed deployments
- [ ] Add deployment status badge to README.md

## Phase 12: Document Your Setup

- [ ] Note your app names:
  - Backend: `_________________`
  - Frontend: `_________________` (if separate)

- [ ] Note your resource group: `_________________`
- [ ] Note your deployment date: `_________________`

- [ ] Share setup guide with team members
- [ ] Add to team wiki/documentation

## Post-Setup Tasks

### Regular Maintenance

- [ ] Monitor deployment logs weekly
- [ ] Check Azure costs monthly
- [ ] Update frameworks and dependencies quarterly
- [ ] Review and update CORS settings as needed

### Future Enhancements

- [ ] Set up deployment slots for zero-downtime deploys
- [ ] Add Azure Key Vault for secrets
- [ ] Set up automated backups
- [ ] Add performance monitoring
- [ ] Implement automated testing in workflow

## Testing the Workflow

After setup, test by making changes:

- [ ] Edit a file (e.g., change product name in backend)
- [ ] Commit: `git commit -am "Test CI/CD"`
- [ ] Push: `git push origin main`
- [ ] Watch Actions tab
- [ ] Wait for deployment
- [ ] Verify change is live on Azure

## Support & Help

If you get stuck:

1. Check `.github/WORKFLOWS_README.md` - **Complete guide**
2. Check `DEPLOYMENT.md` - **Setup instructions**
3. Check GitHub Actions logs - **See exact error**
4. Check Azure Portal logs - **See deployment status**
5. Verify all phases above - **Ensure nothing skipped**

## Checklist Summary

**Total Steps:** 70+

**Estimated Time:** 30-45 minutes

**Expected Outcome:**
- Automated builds on every push to main
- Automatic deployment to Azure App Service
- Working application at azurewebsites.net URL
- Secure deployment using GitHub Secrets

---

## Quick Reference

### Key URLs

- GitHub Repo: `https://github.com/YOUR_USERNAME/azure-webapp-poc`
- GitHub Actions: `https://github.com/YOUR_USERNAME/azure-webapp-poc/actions`
- Azure Portal: `https://portal.azure.com`
- Backend App: `https://my-backend-app.azurewebsites.net`
- Frontend App: `https://my-frontend-app.azurewebsites.net` (Option B only)

### Key Commands

```bash
# Check workflow syntax
python3 -c "import yaml; yaml.safe_load(open('.github/workflows/deploy.yml'))"

# Monitor Azure logs
az webapp log tail --name my-backend-app --resource-group my-rg

# Test API
curl https://my-backend-app.azurewebsites.net/api/products
```

### Key Files

- `.github/workflows/deploy.yml` - Main workflow
- `.github/workflows/deploy-separate.yml` - Alternative workflow
- `backend/Program.cs` - Backend configuration
- `backend/appsettings.Production.json` - Production settings

---

**Status:** ☐ Not Started | ☐ In Progress | ☐ Complete ✓
