﻿using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Tuhu.Extensions.Logging.Structured.Kafka
{
    public static class LoggingBuilderExtensions
    {
        public static IStructuredLoggingBuilder<KafkaLoggingOptions> AddKafka(this ILoggingBuilder builder, string kafkaServer, string topic) => builder.AddKafka(KafkaConstants.Kafka, kafkaServer, topic);

        public static IStructuredLoggingBuilder<KafkaLoggingOptions> AddKafka(this ILoggingBuilder builder, string name, string kafkaServer, string topic)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrWhiteSpace(kafkaServer)) throw new ArgumentNullException(nameof(kafkaServer));
            if (string.IsNullOrWhiteSpace(topic)) throw new ArgumentNullException(nameof(topic));

            return builder.AddKafka(name, options =>
            {
                options.Topic = topic;
                options.ProducerConfig.BootstrapServers = kafkaServer;
            });
        }

        public static IStructuredLoggingBuilder<KafkaLoggingOptions> AddKafka(this ILoggingBuilder builder,
            Action<KafkaLoggingOptions>? configureAction = null) =>
            builder.AddKafka(KafkaConstants.Kafka, configureAction);

        public static IStructuredLoggingBuilder<KafkaLoggingOptions> AddKafka(this ILoggingBuilder builder, string name,
            Action<KafkaLoggingOptions>? configureAction = null)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
            if (configureAction != null) builder.Services.Configure(name, configureAction);

            builder.Services.Configure<KafkaLoggingOptions>(name, options =>
            {
                options.ProducerConfig.CompressionType = CompressionType.Gzip;
                options.ProducerConfig.EnableDeliveryReports = false;
            });

            return builder.AddStructuredLog<KafkaLoggingOptions>(name)
                .SetOutput((options, provider) => new KafkaOutput(options));
        }
    }
}
