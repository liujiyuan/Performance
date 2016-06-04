FROM microsoft/dotnet:1.0.0-rc2-core

# Set the Working Directory
WORKDIR /app

# Configure the listening port to 80
ENV ASPNETCORE_URLS http://*:5000
EXPOSE 80

# Copy the app
COPY .testpublish/ /app

# Start the app
ENTRYPOINT dotnet HelloWorldMvc.dll

