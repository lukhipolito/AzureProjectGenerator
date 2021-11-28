# Introdução

Template para Azure Pipelines para execução dos passos para o build de aplicação em .NET core

# Uso

## Referência do template

```yml
resources:
  repositories:
    - repository: dotnettemplate
      type: git  
      name: WizPipelines/dotnetcore-template
```

## Uso do template

```yml
steps:
  - template: cicd.yml@dotnettemplate
    parameters:
      azSubscriptionUAT: '{nome da subscription de UAT}'
      azSubscriptionPRD: '{nome da subscription de PRD}'
      azResourceName: '{nome da api}'
      appType: 'apiApp'
      dotnetversion: '3.x'
      buildProject: **/*[API].csproj
      buildConfiguration: Release
      testProject: **/*[Tt]ests/*.csproj
      nugetfeed: 09b2821a-2950-4eff-a722-dbc8adf4da55
```

# Parâmetros
```yml
parameters:

# Nome da subscription do azure que será criado o recurso em UAT
- name: azSubscriptionUAT
  default: null
  type: string

# Nome da subscription do azure que será criado o recurso em PRD
- name: azSubscriptionPRD
  default: null
  type: string

# Nome do web app de homlogação.
# Deve ser usado se o nome o webApp não seguir o formato {azResourceName}-hml-api
- name: azWebAppNameUAT
  default: ""
  type: string

# Nome do web app de produção.
# Deve ser usado se o nome o webApp não seguir o formato {azResourceName}-prd-api
- name: azWebAppNamePRD
  default: ""
  type: string

# Nome da api no azure que fará parte da url
- name: azResourceName
  default: null
  type: string

# Tipo do recurso que será criado no azure
- name: appType
  default: 'apiApp'
  type: string

# Versão do framework .net core que a plicação utliza
- name: dotNetVersion
  default: '3.x'
  type: string

# Expressão de refêrencia para o projeto principal
- name: buildProject
  default: '**/*[API].csproj'
  type: string

# Configuração do em que o build vai ser executado
- name: buildConfiguration
  default: 'Release'
  type: string
  values:
  - Release
  - Debug

# Expressão de refêrencia para o projeto de teste
- name: testProject
  default: '**/*[Tt]ests.csproj'
  type: string

# Chave do feed nuget hospedado no azure devops.
- name: nugetFeed
  default: '09b2821a-2950-4eff-a722-dbc8adf4da55'
  type: string

# Define qual o job, stage precisa ocorrer com sucesso antes de dexecutar esse job
- name: depends
  default: null
  type: object

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

```