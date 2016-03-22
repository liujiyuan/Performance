pushd %~dp0
msbuild /p:DeployOnBuild=true /p:PublishProfile=File /p:PublishUrl=.\artifacts\precompiled /p:PrecompileBeforePublish=True
msbuild /p:DeployOnBuild=true /p:PublishProfile=File /p:PublishUrl=.\artifacts\no-precompiled /p:PrecompileBeforePublish=False
popd

ECHO Published to %~dp0artifacts