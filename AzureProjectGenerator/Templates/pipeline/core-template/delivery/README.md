# Template de pipelines de delivery
## 1. WebApp
### 1.1. Parâmetros
``` yml
parameters:
    # environment do Azure DevOps com as regras de deploy
    - name: environment
        type: string
    # Subscription do Azure
    - name: azSubscription
        type: string
    # Nome do WebApp
    - name: azWebApp
        type: string
    # Tipo do WebApp (webApp, webAppLinux, webAppContainer, functionApp, functionAppLinux, functionAppContainer, apiApp, mobileApp )
    - name: azAppType
        default: 'webApp'
        type: string
    # Caminho do zip criado pelo build
    - name: deployPackage
        default: '$(Pipeline.Workspace)/drop/**/*.zip'
        type: string
    # Define qual o job, stage precisa ocorrer com sucesso antes de dexecutar esse job
    - name: depends
        default: null
        type: object
    # Define parametros do settings para passar ao ambiente do azure. ex: -ASPNETCORE_ENVIRONMENT Staging
    - name: appSettings
      default: null
      type: string
    # Define parametros do settings para passar ao ambiente do azure. ex: -linuxFxVersion: node|6.11
    - name: configurationSettings
      default: null
      type: string
    # Injetar services/containers no pipeline
    - name: services
      default: null
      type: object
```
### 1.2. Como utilizar
Adicione o caminho do template no seu pipeline:
``` yml
resources:
  repositories:
    - repository: coretemplate
      type: git  
      name: WizPipelines/core-template
```

Inclua os jobs dentro do seu job, abaixo um exemplo do speed-api com todos os parâmetros:
``` yml
jobs:
    - template: delivery/webapp.yml@coretemplate
      parameters:
        depends: Build
        environment: 'production'
        azSubscription: 'BPO'
        azWebApp: '$(azResourceName)-prd-api'
        azAppType: 'apiApp'
        deployPackage: '$(Pipeline.Workspace)/drop/**/*.zip'
        AppSettings: ${{parameters.appSettings}}
        ConfigurationSettings: ${{parameters.configurationSettings}}
```
## 2. BlobStorage
### 2.1. Parâmetros
``` yml
parameters:
    # environment do Azure DevOps com as regras de deploy
    - name: environment
        type: string
    # Subscription do Azure
    - name: azSubscription
        type: string
    # Nome do Storage
    - name: azStorage
        type: string
    # Caminho do zip criado pelo build
    - name: deployPath
        default: '$(Pipeline.Workspace)/${{parameters.environment}}/*'
        type: string
    # Define qual o job, stage precisa ocorrer com sucesso antes de dexecutar esse job
    - name: depends
        default: null
        type: object
    # Injetar services/containers no pipeline
    - name: services
      default: null
      type: object
```
### 2.2. Como utilizar
Adicione o caminho do template no seu pipeline:
``` yml
resources:
  repositories:
    - repository: coretemplate
      type: git  
      name: WizPipelines/core-template
```

Inclua os jobs dentro do seu job, abaixo um exemplo do bbxweb com todos os parâmetros:
``` yml
jobs:
    - template: delivery/azureblob.yml@coretemplate
      parameters:
        environment: 'production'
        azSubscription: 'AmbienteCorporativoExterior'
        azStorage: 'bbxprdstg'
        azAppType: 'apiApp'
        dependsOn: [Build]
        deployPath: '$(Pipeline.Workspace)/production/*'
```

## 3. IIS
### 3.1. Parâmetros
``` yml
parameters:
  # environment do Azure DevOps com as regras de deploy
  - name: environment
    type: string

  # Nome da VM com o IIS
  - name: iisMachine
    default: 'srva022'
    type: string

  # Nome do website
  - name: websiteName
    type: string

  # URL do website
  - name: websiteUrl
    type: string
    default: ''

  # Tipo de deploy no IIS
  - name: iisDeploymentType
    type: string
    default: 'IISWebsite'
    values:
      - IISWebsite
      - IISWebApplication
      - IISVirtualDirectory
      - IISApplicationPool

  # Caminho dos arquivos no IIS
  - name: websitePhysicalPath
    type: string
    default: 'F:\Arquivo_IIS\wwwroot'

  # Versão do .NET Framework para execução da aplicação
  - name: dotNetVersion
    type: string
    default: 'v4.0'
    values:
      - v4.0
      - v2.0
      - No Managed Code

  # Modo do pipeline
  - name: pipeLineMode
    type: string
    default: 'Integrated'
    values:
      - Integrated
      - Classic

  # Thumbprint do certificado SSL (padrão *.wizsolucoes.com.br)
  - name: sslThumbprint
    type: string
    default: 'ba112dfdbbe73c842f088dfebf0b0d5f8898eea0'

  # Caminho do zip criado pelo build
  - name: deployPackage
    default: '$(System.DefaultWorkingDirectory)\**\staging\*API.zip'
    type: string

  # Realizar substituição das variáveis dos arquivos de configuração
  - name: xmlVariableSubstitution
    default: true
    type: boolean

  # Realizar transformação de XML no arquivo de configuração
  - name: xmlTransformation
    default: false
    type: boolean

  # Define qual o job, stage precisa ocorrer com sucesso antes de dexecutar esse job
  - name: depends
    default: null
    type: object

  # Injetar services/containers no pipeline
  - name: services
    default: ""
    type: object
```
### 3.2. Como utilizar
Adicione o caminho do template no seu pipeline:
``` yml
resources:
  repositories:
    - repository: coretemplate
      type: git  
      name: WizPipelines/core-template
```

Inclua os jobs dentro do seu job, abaixo um exemplo do bbxweb com todos os parâmetros:
``` yml
jobs:
    - template: delivery/iis.yml@coretemplate
      parameters:
        environment: 'production'
        websiteName: 'integracaoad-prd-api'
        dependsOn: [Build]
        deployPackage: '$(System.DefaultWorkingDirectory)\**\production\*API.zip'
```