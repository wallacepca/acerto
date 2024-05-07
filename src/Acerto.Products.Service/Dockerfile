FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR "/app"

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

COPY . .
RUN dotnet restore "/app/src/Acerto.Products.Service/Acerto.Products.Service.csproj" --runtime linux-musl-x64
RUN dotnet publish "/app/src/Acerto.Products.Service/Acerto.Products.Service.csproj" -c Release -o /app/publish \
      --no-restore \
      --runtime linux-musl-x64 \
      --self-contained true \
      /p:DebugType=None \
      /p:DebugSymbols=false \  
      /p:PublishSingleFile=true

FROM mcr.microsoft.com/dotnet/runtime-deps:8.0-alpine AS final
EXPOSE 8080
EXPOSE 8081
WORKDIR "/app"

ARG VCS_REF
ARG VCS_URL

LABEL org.label-schema.vcs-ref=$VCS_REF \
      org.label-schema.vcs-url=$VCS_URL

COPY --from=build /app/publish ./
ENTRYPOINT ["./Acerto.Products.Service"]