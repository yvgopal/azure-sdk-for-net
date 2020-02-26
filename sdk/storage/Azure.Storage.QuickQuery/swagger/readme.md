# Blob Storage
> see https://aka.ms/autorest

## Configuration
``` yaml
# Generate blob storage
input-file: https://raw.githubusercontent.com/Azure/azure-rest-api-specs/storage-dataplane-preview/specification/storage/data-plane/Microsoft.BlobStorage/preview/2019-12-12/blob.json
output-folder: ../src/Generated
clear-output-folder: false

# Use the Azure C# Track 2 generator
# use: C:\src\Storage\Swagger\Generator
# We can't use relative paths here, so use a relative path in generate.ps1
azure-track2-csharp: true
```

## Customizations for Track 2 Generator
See the [AutoRest samples](https://github.com/Azure/autorest/tree/master/Samples/3b-custom-transformations)
for more about how we're customizing things.

### x-ms-code-generation-settings
``` yaml
directive:
- from: swagger-document
  where: $.info["x-ms-code-generation-settings"]
  transform: >
    $.namespace = "Azure.Storage.QuickQuery";
    $["client-name"] = "QuickQueryRestClient";
    $["client-extensions-name"] = "QuickQueryExtensions";
    $["client-model-factory-name"] = "QuickQueryModelFactory";
    $["x-az-skip-path-components"] = true;
    $["x-az-include-sync-methods"] = true;
    $["x-az-public"] = false;
```

### Remove extra consumes/produces values
To avoid an arbitrary limitation in our generator
``` yaml
directive:
- from: swagger-document
  where: $.consumes
  transform: >
    return ["application/xml"];
- from: swagger-document
  where: $.produces
  transform: >
    return ["application/xml"];
```

### Url
``` yaml
directive:
- from: swagger-document
  where: $.parameters.Url
  transform: >
    $["x-ms-client-name"] = "resourceUri";
    $.format = "url";
    $["x-az-trace"] = true;
```

### Ignore common headers
``` yaml
directive:
- from: swagger-document
  where: $["x-ms-paths"]..responses..headers["x-ms-request-id"]
  transform: >
    $["x-az-demote-header"] = true;
- from: swagger-document
  where: $["x-ms-paths"]..responses..headers["x-ms-version"]
  transform: >
    $["x-az-demote-header"] = true;
- from: swagger-document
  where: $["x-ms-paths"]..responses..headers["Date"]
  transform: >
    $["x-az-demote-header"] = true;
- from: swagger-document
  where: $["x-ms-paths"]..responses..headers["x-ms-client-request-id"]
  transform: >
    $["x-az-demote-header"] = true;
```

### Clean up Failure response
``` yaml
directive:
- from: swagger-document
  where: $["x-ms-paths"]..responses.default
  transform: >
    delete $.headers;
    $["x-az-response-name"] = "StorageErrorResult";
    $["x-az-create-exception"] = true;
    $["x-az-public"] = false;
```

### Delete all operations Quick Query
``` yaml
directive:
- from: swagger-document
  where: $["x-ms-paths"]
  transform: >
    return {
         "/{containerName}/{blob}?comp=query": $[ "/{containerName}/{blob}?comp=query"],
    };
```

### Delete all parameters except those for Quick Query
``` yaml
directive:
- from: swagger-document
  where: $.parameters
  transform: >
    $ = {
        "QueryRequest": $.QueryRequest,
        "Snapshot": $.Snapshot,
        "Timeout": $.Timeout,
        "LeaseIdOptional": $.LeaseIdOptional,
        "EncryptionKey": $.EncryptionKey,
        "EncryptionKeySha256": $.EncryptionKeySha256,
        "EncryptionAlgorithm": $.EncryptionAlgorithm,
        "IfModifiedSince": $.IfModifiedSince,
        "IfUnmodifiedSince": $.IfUnmodifiedSince,
        "IfMatch": $.IfMatch,
        "IfNoneMatch": $.IfNoneMatch,
        "ApiVersionParameter": $.ApiVersionParameter,
        "ClientRequestId": $.ClientRequestId,
        "Url": $.Url
    };

    return $;
```

### Delete all definitions except those for Quick Query
``` yaml
directive:
- from: swagger-document
  where: $.definitions
  transform: >
    $ = {
        "QueryRequest": $.QueryRequest,
        "QuickQueryFormat": $.QuickQueryFormat,
        "QuickQuerySerialization": $.QuickQuerySerialization,
        "QuickQueryType": $.QuickQueryType,
        "DelimitedTextConfiguration": $.DelimitedTextConfiguration,
        "JsonTextConfiguration": $.JsonTextConfiguration,
        "StorageError": $.StorageError
    };

    return $;
```

### Hide BlobQuickQueryResult
``` yaml
directive:
- from: swagger-document
  where: $["x-ms-paths"]["/{containerName}/{blob}?comp=query"]
  transform: >
    $.post.responses["200"]["x-az-public"] = false;
    $.post.responses["206"]["x-az-public"] = false;
```

### Hide QueryRequest
``` yaml
directive:
- from: swagger-document
  where: definitions.QueryRequest
  transform: >
    $["x-az-public"] = false;
```

### Hide QuickQueryFormat
``` yaml
directive:
- from: swagger-document
  where: definitions.QuickQueryFormat
  transform: >
    $["x-az-public"] = false;
```

### Hide QuickQuerySerialization
``` yaml
directive:
- from: swagger-document
  where: definitions.QuickQuerySerialization
  transform: >
    $["x-az-public"] = false;
```

### Hide JsonTextConfiguration
``` yaml
directive:
- from: swagger-document
  where: definitions.JsonTextConfiguration
  transform: >
    $["x-az-public"] = false;
    $["x-ms-client-name"] = "JsonTextConfigurationInternal";
```

### Hide DelimitedTextConfiguration
``` yaml
directive:
- from: swagger-document
  where: definitions.DelimitedTextConfiguration
  transform: >
    $["x-az-public"] = false;
    $["x-ms-client-name"] = "DelimitedTextConfigurationInternal";
```

### Hide Error models
``` yaml
directive:
- from: swagger-document
  where: $.definitions.StorageError
  transform: >
    $["x-az-public"] = false;
    $.properties.Code = { "type": "string" };
```

### Treat the API version as a parameter instead of a constant
``` yaml
directive:
- from: swagger-document
  where: $.parameters.ApiVersionParameter
  transform: >
    delete $.enum
```
