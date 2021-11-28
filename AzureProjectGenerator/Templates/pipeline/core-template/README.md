# Template de CI de CD 
## 1. Parâmetros
``` yml
parameters:
  # Technologia cujo pipeline de CI/CD que deve ser usado
  - name: technology
    type: string
    values:
    - angular
    - angularjs
    - apim
    - dotnetcore
    - dotnetframework
    - infra
    - node-app
    - node-package
    - protheus
    - reactjs
    - reactnative
    - salesforce
    - sqlserver
    - vue

  # Tipo de deployment. Pode ser 'webapp' ou 'storage'
  - name: deploymentType
    type: string
    default: 'webapp'
    values:
    - webapp
    - storage
    - iis

  # Prefixo do nome do recurso de deployment. Por exemplo:
  # se os nomes dos seus webapps forem xpto-hml-web e xpto-prd-web
  # ou dos seus storages forem xptohtmlstg e xptoprdstg,
  # o azResourceName seria 'xpto'
  - name: azResourceName
    default: null
    type: string

  # AppInsights Intrumentation Key
  - name: instrumentationKey
    default: ""
    type: string

  # Nome do web app de homlogação.
  # Deve ser usado somente se o deploymentType for 'webapp' e 
  # se o nome o webApp não seguir o formato {azResourceName}-hml-web
  - name: azWebAppNameUAT
    default: ""
    type: string

  # Nome do web app de sandbox.
  # Deve ser usado somente se o deploymentType for 'webapp' e 
  # se o nome o webApp não seguir o formato {azResourceName}-sb-web
  - name: azWebAppNameSB
    default: ""
    type: string

  # Nome do web app de produção.
  # Deve ser usado somente se o deploymentType for 'webapp' e 
  # se o nome o webApp não seguir o formato {azResourceName}-prd-web
  - name: azWebAppNamePRD
    default: ""
    type: string

  # Assinatura Azure de desenvolvimento
  - name: azSubscriptionDEV
    default: ""
    type: string

  # Assinatura Azure de homolgação
  - name: azSubscriptionUAT
    default: 'AmbientesDevHml'
    type: string

  # Assinatura Azure de sandbox. O valor é obrigatório para acionar o deploy para sandbox
  - name: azSubscriptionSB
    default: ""
    type: string

  # Assinatura Azure de produção
  - name: azSubscriptionPRD
    default: 'AmbienteCorporativoExterior'
    type: string

  # Injetar services/containers no pipeline de build
  - name: buildServices
    default: null
    type: object

  # Injetar services/containers no pipeline de release
  - name: releaseServices
    default: null
    type: object

  # Define parametros para passar ao ambiente do azure em UAT. ex: -API_URL example.com
  - name: appSettingsUAT
    default: ""
    type: string

  # Define parametros para passar ao ambiente do azure em produção. ex: -API_URL example.com
  - name: appSettingsPRD
    default: ""
    type: string

  # Nome da service connection do registro Docker para deploys em container
  - name: dockerContainerRegistry
    default: "wizhub"
    type: string

  # Nome do repostório com o registro de container que corresponde à service connection especificado em `dockerContainerRegistry`
  - name: dockerImageRepo
    default: ""
    type: string

  - name: verbose
    default: false
    type: boolean 

  ######################
  # Parâmetros Angular #
  ######################

  # Buildar usando compliação "Ahead of Time".
  - name: angularBuildAOT
    default: true
    type: boolean

  # Nome do projeto principal, para aplicações com estrutura de workspace muti-project
  - name: angularProjectName
    default: ""
    type: string

  #############################
  # Parâmetros Api Management #
  #############################

  # Resource Group do apim 
  - name: terraformStateResourceGroup
    default: ""
    type: string

  # Blob storage do apim 
  - name: terraformStateStorageAccount
    default: ""
    type: string

  # State do apim 
  - name: terraformStatePath
    default: ""
    type: string

  # Parametros extras pro terraform 
  - name: terraformParams
    default: ""
    type: string

  # Diretorio com o script do terraform 
  - name: terraformScriptsPath
    default: ""
    type: string

  #####################
  # Parâmetros Vue.js #
  #####################

  # Argumentos para o script de build de UAT
  - name: vueBuildArgumentsUAT
    default: ""
    type: string

  # Argumentos para o script de build de produção
  - name: vueBuildArgumentsPRD
    default: ""
    type: string

  # Arquivos que devem ser copiados para a pasta dist depois do build
  - name: vueComplementaryDistFiles
    default: ""
    type: string

  ########################
  # Parâmetros .Net Core #
  ########################

  # Tipo do recurso que será criado no azure
  - name: dotnetcoreAppType
    default: 'apiApp'
    type: string

  # Versão do framework .net core que a aplicação utliza
  - name: dotnetcoreDotNetVersion
    default: '3.x'
    type: string

  # Expressão de refêrencia para o projeto principal
  - name: dotnetcoreBuildProject
    default: '**/*[API].csproj'
    type: string

  # Configuração do em que o build vai ser executado
  - name: dotnetcoreBuildConfiguration
    default: 'Release'
    type: string
    values:
    - Release
    - Debug

  # Expressão de refêrencia para o projeto de teste
  - name: dotnetcoreTestProject
    default: '**/*[Tt]ests.csproj'
    type: string

  # Chave do feed nuget hospedado no azure devops.
  - name: dotnetcoreNugetFeed
    default: '09b2821a-2950-4eff-a722-dbc8adf4da55'
    type: string

  # IIS vm PRD
  - name: iisMachinePRD
    default: ""
    type: string

  # IIS vm UAT
  - name: iisMachineUAT
    default: ""
    type: string

  # IIS folder wwwroot
  - name: websitePhysicalPath
    default: '%SystemDrive%\inetpub\wwwroot'
    type: string

  # IIS bind url
  - name: bindings
    default: true
    type: boolean

  ##############################
  # Parâmetros Node.js Package #
  ##############################

  # Define se a publicaçao npm é para um registro externo ou um feed
  - name: npmPublishRegistry
    default: ""
    type: string

  # Define o feed de publicação npm quando npmPublishRegistry == useFeed
  - name: npmPublishFeed
    default: ""
    type: string

  # Define uma service connection para publicação npm
  - name: npmCustomEndpoint
    default: ""
    type: string

  # Define se o comando de build deve ser executado
  - name: npmRunBuild
    default: false
    type: boolean

  #######################
  # Parâmetros Protheus #
  #######################

  # Usuário de compilação
  - name: protheusCompilerUser
    default: ''
    type: string

  # Senha de compilação
  - name: protheusCompilerPassword
    default: ''
    type: string

  # Caminho dos arquivos com os includes para compilar os fontes
  - name: protheusIncludePath
    default: '$(Build.SourcesDirectory)/resources/includes'
    type: string

  # Caminho dos arquivos fontes
  - name: protheusProgramPath
    default: '$(Build.SourcesDirectory)/src'
    type: string

  # Nome do servidor de homologação
  - name: protheusHmlServer
    default: 'srvsh021'
    type: string
  
  # Nome do servidor de producao
  - name: protheusPrdServer
    default: 'srvsh021'
    type: string

  # Porta do servidor de homologação
  - name: protheusHmlServerPort
    default: 99000
    type: number

  # Porta do servidor de produção
  - name: protheusPrdServerPort
    default: 35999
    type: number

  # Porta do servidor de rest homologacao
  - name: protheusHmlRestPort
    default: 2593
    type: number
  
  # Porta do servidor de rest producao
  - name: protheusPrdRestPort
    default: 2593
    type: number

  # Nome do ambiente de compilação de homologação
  - name: protheusHmlEnvironmentName
    default: 'HOM_12127_COMPILER'
    type: string

  # Nome do ambiente de compilação de produção
  - name: protheusPrdEnvironmentName
    default: 'HOM_12127_MASTER_COMPILER'
    type: string

  ###########################
  # Parâmetros React Native #
  ###########################

  # Nome do Keystore que usado para assinar o apk para Google Play
  - name: reactnativeGooglePlayKeyStoreName
    default: ""
    type: string
  
  #########################
  # Parâmetros SQL Server #
  #########################

  # Nome do servidor SQLServer de DEV
  - name: sqlserverSqlServernameDEV
    default: ""
    type: string

  # Nome do servidor SQLServer de UAT
  - name: sqlserverSqlServernameUAT
    default: ""
    type: string

  # Nome do servidor SQLServer de PRD
  - name: sqlserverSqlServernamePRD
    default: ""
    type: string

  # Nome da base de dados do SQLServer de DEV
  - name: sqlserverDatabaseNameDEV
    default: ""
    type: string
  
  # Nome da base de dados do SQLServer de UAT
  - name: sqlserverDatabaseNameUAT
    default: ""
    type: string
  
  # Nome da base de dados do SQLServer de PRD
  - name: sqlserverDatabaseNamePRD
    default: ""
    type: string
  
  # Nome do usuário do AzureAD
  - name: sqlserverUsername
    default: ""
    type: string
  
  # Senha do usuário do AzureAD 
  - name: sqlserverPassword
    default: ""
    type: string
```
## 2. Como utilizar
Adicionar o caminho do template no seu pipeline:
``` yml
resources:
  repositories:
    - repository: coretemplate
      type: git
      name: WizPipelines/core-template
```

Extender o template (exemplo:)
```yml
extends:
  template: main.yml@coretemplate
  parameters:
    technology: 'angular'
    deploymentType: 'webapp'
    azResourceName: 'conexaoconseg'
    instrumentationKey: 'chave-de-instrumentação-do-app'
```