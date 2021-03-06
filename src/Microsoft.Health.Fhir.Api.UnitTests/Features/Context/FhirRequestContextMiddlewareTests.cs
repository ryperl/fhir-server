﻿// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Health.Fhir.Api.Features.Context;
using Microsoft.Health.Fhir.Core.Features.Context;
using NSubstitute;
using Xunit;

namespace Microsoft.Health.Fhir.Api.UnitTests.Features.Context
{
    public class FhirRequestContextMiddlewareTests
    {
        [Fact]
        public async Task GivenAnHttpRequest_WhenExecutingFhirRequestContextMiddleware_ThenCorrectRequestTypeShouldBeSet()
        {
            IFhirRequestContext fhirRequestContext = await SetupAsync(CreateHttpContext());

            Assert.NotNull(fhirRequestContext.RequestType);

            Assert.Equal(ValueSets.AuditEventType.RestFulOperation.System, fhirRequestContext.RequestType.System);
            Assert.Equal(ValueSets.AuditEventType.RestFulOperation.Code, fhirRequestContext.RequestType.Code);
        }

        [Fact]
        public async Task GivenAnHttpRequest_WhenExecutingFhirRequestContextMiddleware_ThenCorrectUriShouldBeSet()
        {
            IFhirRequestContext fhirRequestContext = await SetupAsync(CreateHttpContext());

            Assert.Equal(new Uri("https://localhost:30/stu3/Observation?code=123"), fhirRequestContext.Uri);
        }

        [Fact]
        public async Task GivenAnHttpRequest_WhenExecutingFhirRequestContextMiddleware_ThenCorrectBaseUriShouldBeSet()
        {
            IFhirRequestContext fhirRequestContext = await SetupAsync(CreateHttpContext());

            Assert.Equal(new Uri("https://localhost:30/stu3"), fhirRequestContext.BaseUri);
        }

        [Fact]
        public async Task GivenAnHttpContextWithPrincipal_WhenExecutingFhirRequestContextMiddleware_ThenPrincipalShouldBeSet()
        {
            HttpContext httpContext = CreateHttpContext();

            var principal = new ClaimsPrincipal();

            httpContext.User = principal;

            IFhirRequestContext fhirRequestContext = await SetupAsync(httpContext);

            Assert.Same(principal, fhirRequestContext.Principal);
        }

        private async Task<IFhirRequestContext> SetupAsync(HttpContext httpContext)
        {
            var fhirRequestContextAccessor = Substitute.For<IFhirRequestContextAccessor>();
            var fhirContextMiddlware = new FhirRequestContextMiddleware(next: (innerHttpContext) => Task.CompletedTask);
            string Provider() => Guid.NewGuid().ToString();

            await fhirContextMiddlware.Invoke(httpContext, fhirRequestContextAccessor, Provider);

            Assert.NotNull(fhirRequestContextAccessor.FhirRequestContext);

            return fhirRequestContextAccessor.FhirRequestContext;
        }

        private HttpContext CreateHttpContext()
        {
            HttpContext httpContext = new DefaultHttpContext();

            httpContext.Request.Method = "GET";
            httpContext.Request.Scheme = "https";
            httpContext.Request.Host = new HostString("localhost", 30);
            httpContext.Request.PathBase = new PathString("/stu3");
            httpContext.Request.Path = new PathString("/Observation");
            httpContext.Request.QueryString = new QueryString("?code=123");

            return httpContext;
        }
    }
}
