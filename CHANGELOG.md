# Changelog
All notable changes to this project will be documented in this file.

## [Update to AspNetCore v3.1.2]  (2020-03-05)

### Summary
- All packages **updated to the latest version**.
- **Code improvement** and **bug fixes**.
- **AutoMapper** v9.0.0 changes applied and static `Mapper` removed.
- **Swashbuckle** new changes applied.
- **NETCore 3.x** and **ASPNETCore 3.x** and **EFCore 3.x** new changes applied.

### Details

- Because of updates in `AutoMapper` api and removing the static `Mapper` class, `IMapper` passed to all controller and services that uses mapping. (for example [OldPostsController.cs](https://github.com/dotnetzoom/AspNetCore-WebApi-Course/commit/512375f4768b4ed08f0e03cf58e14ac392cd1c3d#diff-eaab1d7550c3c321acce8aed23408f31))

- Mapping implementation changed in [BaseDto.cs](https://github.com/dotnetzoom/AspNetCore-WebApi-Course/commit/512375f4768b4ed08f0e03cf58e14ac392cd1c3d#diff-e7ce6de2fd70a2e3cffd18f3ada9e392)

- Mapping configuration changed in [AutoMapperConfiguration.cs](https://github.com/dotnetzoom/AspNetCore-WebApi-Course/commit/512375f4768b4ed08f0e03cf58e14ac392cd1c3d#diff-9eac783364651f0bb7476af1a68abc6c)

- Package `Microsoft.EntityFrameworkCore.Tools` added. We need it since EFCore v3.x to use migration command in Nuget Package Manager Console.

- Because of EFCore3.x api changes, `entityType.Relational().TableName` changed to `entityType.GetTableName()` and `property.Relational().DefaultValueSql` changed to `property.SetDefaultValueSql()` in [ModelBuilderExtensions.cs](https://github.com/dotnetzoom/AspNetCore-WebApi-Course/commit/512375f4768b4ed08f0e03cf58e14ac392cd1c3d#diff-becd12d58ff3b004786ad98ed04ab5e9)

- `ConfigureWarnings` removed in [ServiceCollectionExtensions.cs](https://github.com/dotnetzoom/AspNetCore-WebApi-Course/commit/512375f4768b4ed08f0e03cf58e14ac392cd1c3d#diff-92fe43494731aa038324b5b4b89b795cL37) because automatic client evaluation is no longer supported and this event is no longer generated.

- `ValueTask` replaced by `Task` in `GetByIdAsync` method of [Repository](https://github.com/dotnetzoom/AspNetCore-WebApi-Course/commit/512375f4768b4ed08f0e03cf58e14ac392cd1c3d#diff-94b60a9279541f7ec2317bd1ccb39e10R28)

- Package `Swashbuckle.AspNetCore.Examples` replaced by `Swashbuckle.AspNetCore.Filters` and so their namespaces.

- `IExamplesProvider` changed to generic version `IExamplesProvider<T>` (see [CreateUserResponseExample](https://github.com/dotnetzoom/AspNetCore-WebApi-Course/compare/AspNetCore2.1...master#diff-df20c65f37a9123d4f975cbd679a0b58L155))

- Package `NLog.Targets.Sentry2` replaced by `Sentry.NLog` and their related code changed in [Program.cs](https://github.com/dotnetzoom/AspNetCore-WebApi-Course/commit/512375f4768b4ed08f0e03cf58e14ac392cd1c3d#diff-c47d99eda2de424251000d1108618003) and its [nlog.config](https://github.com/dotnetzoom/AspNetCore-WebApi-Course/commit/512375f4768b4ed08f0e03cf58e14ac392cd1c3d#diff-20fb3fba6421f6afb2c8c7e29ba38ef9) changed a little.

- Due to model changes in OpenApi(Swagger), these files changed [ApplySummariesOperationFilter.cs](https://github.com/dotnetzoom/AspNetCore-WebApi-Course/commit/512375f4768b4ed08f0e03cf58e14ac392cd1c3d#diff-4bec3d908f7d545fcef8aba09fcff33f), [RemoveVersionParameters.cs](https://github.com/dotnetzoom/AspNetCore-WebApi-Course/commit/512375f4768b4ed08f0e03cf58e14ac392cd1c3d#diff-2cb5e55c2bd5965637f6157197bbcca8), [SetVersionInPaths.cs](https://github.com/dotnetzoom/AspNetCore-WebApi-Course/commit/512375f4768b4ed08f0e03cf58e14ac392cd1c3d#diff-1cc6c72fa90caa6d8f9d0f68b5426ca1), [UnauthorizedResponsesOperationFilter.cs](https://github.com/dotnetzoom/AspNetCore-WebApi-Course/commit/512375f4768b4ed08f0e03cf58e14ac392cd1c3d#diff-6ff134c9756202d7c244ca64c4334d75)

- Swagger configuration in [SwaggerConfigurationExtensions.cs](https://github.com/dotnetzoom/AspNetCore-WebApi-Course/commit/512375f4768b4ed08f0e03cf58e14ac392cd1c3d#diff-04b7fe07540f95c0feef71762d4427e2) changed due to swagger new updates

- Implementation of [ApiResultFilterAttribute.cs](https://github.com/dotnetzoom/AspNetCore-WebApi-Course/commit/512375f4768b4ed08f0e03cf58e14ac392cd1c3d#diff-6666eb9214b52adc50e21618657a5fbe) changed due to ASPNETCore 3.x new updates

- Implementation of `AddMinimalMvc` in [ServiceCollectionExtensions.cs ](https://github.com/dotnetzoom/AspNetCore-WebApi-Course/commit/512375f4768b4ed08f0e03cf58e14ac392cd1c3d#diff-92fe43494731aa038324b5b4b89b795cL41) method changed since `service.AddControllers()` was introduced.

- `IHostingEnvironment` replaced by `IWebHostEnvironment` because of API deprecated in NETCore 3.x ([here](https://github.com/dotnetzoom/AspNetCore-WebApi-Course/commit/512375f4768b4ed08f0e03cf58e14ac392cd1c3d#diff-9630cb1f7e42938b535ac527b562c5b5) for example)

- The `app.UseMvc()` replaced by `app.UseRouting()` and `app.UseEndpoints()` in [Startup.Configure](https://github.com/dotnetzoom/AspNetCore-WebApi-Course/commit/512375f4768b4ed08f0e03cf58e14ac392cd1c3d#diff-a045957754a1b17bd3f0ba5e21bd0c13R68) method

- Unused namespaces in all projects removed.

[Update to AspNetCore v3.1.2]: https://github.com/dotnetzoom/AspNetCore-WebApi-Course/compare/AspNetCore2.1...master
