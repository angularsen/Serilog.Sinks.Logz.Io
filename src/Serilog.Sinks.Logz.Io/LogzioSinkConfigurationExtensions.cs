// Copyright 2015-2016 Serilog Contributors
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using Elastic.CommonSchema.Serilog;
using Serilog.Configuration;
using Serilog.Events;
using Serilog.Sinks.Logz.Io;
using Serilog.Sinks.Logz.Io.Client;

// ReSharper disable once CheckNamespace
namespace Serilog
{
    /// <summary>
    /// Adds the WriteTo.LogzIo() extension method to <see cref="LoggerConfiguration"/>.
    /// </summary>
    public static class LogzioSinkConfigurationExtensions
    {
        /// <summary>
        /// Adds a sink that sends log events using HTTP POST over the network.
        /// </summary>
        /// <param name="sinkConfiguration">The logger configuration.</param>
        /// <param name="authToken">The token for your logzio account.</param>
        /// <param name="type">Your log type - it helps classify the logs you send.</param>
        /// <param name="useHttps">Specifies to use https (default is true)</param>
        /// <param name="boostProperties">When true, does not add 'properties' prefix.</param>
        /// <param name="dataCenterSubDomain">The logz.io datacenter specific sub-domain to send the logs to. options: "listener" (default, US), "listener-eu" (EU)</param>
        /// <param name="restrictedToMinimumLevel">Specifies minimal level for log events</param>
        /// <param name="batchPostingLimit">The maximum number of events to post in a single batch</param>
        /// <param name="period">The time to wait between checking for event batches</param>
        /// <param name="lowercaseLevel">Set to true to push log level as lowercase</param>
        /// <param name="environment">The environment name, default is empty and not sent to server</param>
        /// <param name="serviceName">The service name, default is empty and not sent to server</param>
        /// <param name="includeMessageTemplate">When true the message template is included in the logs</param>
        /// <param name="port">When specified overrides default port</param>
        /// <returns>Logger configuration, allowing configuration to continue.</returns>
        public static LoggerConfiguration LogzIo(
            this LoggerSinkConfiguration sinkConfiguration,
            string authToken,
            string type,
            bool useHttps = true,
            bool boostProperties = false,
            string dataCenterSubDomain = "listener",
            LogEventLevel restrictedToMinimumLevel = LogEventLevel.Verbose,
            int? batchPostingLimit = null,
            TimeSpan? period = null,
            bool lowercaseLevel = false,
            string? environment = null,
            string? serviceName = null,
            bool includeMessageTemplate = false,
            int? port = null)
        {
            return LogzIo(sinkConfiguration, authToken, type, new LogzioOptions
            {
                UseHttps = useHttps,
                BoostProperties = boostProperties,
                DataCenterSubDomain = dataCenterSubDomain,
                RestrictedToMinimumLevel = restrictedToMinimumLevel,
                BatchPostingLimit = batchPostingLimit,
                Period = period,
                LowercaseLevel = lowercaseLevel,
                Environment = environment,
                ServiceName = serviceName,
                IncludeMessageTemplate = includeMessageTemplate,
                Port = port
            });
        }

        /// <summary>
        /// Adds a sink that sends log events using HTTP POST over the network.
        /// </summary>
        /// <param name="sinkConfiguration">The logger configuration.</param>
        /// <param name="authToken">The token for your logzio account.</param>
        /// <param name="type">Your log type - it helps classify the logs you send.</param>
        /// <param name="options">Logzio configuration options</param>
        /// <returns>Logger configuration, allowing configuration to continue.</returns>
        public static LoggerConfiguration LogzIo(this LoggerSinkConfiguration sinkConfiguration, string authToken, string type, LogzioOptions options = null)
        {
            if (sinkConfiguration == null)
                throw new ArgumentNullException(nameof(sinkConfiguration));

            var client = new HttpClientWrapper();
            var sink = new LogzioSink(client, authToken, type, options ?? new LogzioOptions());
            var restrictedToMinimumLevel = options?.RestrictedToMinimumLevel ?? LogEventLevel.Verbose;

            return sinkConfiguration.Sink(sink, restrictedToMinimumLevel);
        }

        /// <summary>
        /// Adds a sink that sends log events using HTTP POST over the network.
        /// </summary>
        /// <param name="sinkConfiguration">The logger configuration.</param>
        /// <param name="options">Logzio configuration options</param>
        /// <param name="batchPostingLimit"></param>
        /// <param name="period"></param>
        /// <param name="restrictedToMinimumLevel"></param>
        /// <param name="formatterConfiguration"></param>
        /// <returns>Logger configuration, allowing configuration to continue.</returns>
        public static LoggerConfiguration LogzIoEcs(this LoggerSinkConfiguration sinkConfiguration
            , LogzioEcsOptions options
            , int? batchPostingLimit = null
            , TimeSpan? period = null
            , LogEventLevel restrictedToMinimumLevel = LogEventLevel.Warning
            , IEcsTextFormatterConfiguration? formatterConfiguration = null)
        {
            if (sinkConfiguration == null)
                throw new ArgumentNullException(nameof(sinkConfiguration));

            if (options == null)
                throw new ArgumentNullException(nameof(options));

            var sink = new LogzioEcsSink(options, batchPostingLimit, period, null, formatterConfiguration);

            return sinkConfiguration.Sink(sink, restrictedToMinimumLevel);
        }
    }
}
