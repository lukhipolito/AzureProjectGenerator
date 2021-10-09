cd ${templatesPath}										     \
dotnet pack                                                  \
dotnet new -i ./bin/Debug/AzureProjectTemplate.1.0.0.nupkg   \
cd ${output}                                                 \
mkdir ${name}.API                                            \
cd ${name}.API                                               \
dotnet new azureprojectapi -n ${name}                        \
cd ..                                                        \
mkdir ${name}.Domain                                         \
cd ${name}.Domain                                            \
dotnet new azureprojectdomain -n ${name}                     \
cd ..                                                        \
mkdir ${name}.Infra                                          \
cd ${name}.Infra                                             \
dotnet new azureprojectinfra -n ${name}                      \
cd ..                                                        \
dotnet new sln -n ${name}                                    \
dotnet sln add ${name}.API/${name}.API.csproj                \
dotnet sln add ${name}.Domain/${name}.Domain.csproj          \
dotnet sln add ${name}.Infra/${name}.Infra.csproj