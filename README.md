# ASP.NET CORE 服务监控

> - `App.Metrics` 基于 `ASP.NET Core` 监控采集中间件
> - `Grafana` 监控图像组件
> - `InfluxDb` 时序数据库

## Metrics 简介

> Metrics是一个系统性能度量框架，提供了Gauge、Counter、Meter、Histogram、Timer等度量工具类以及Health Check功能。

Gauge (仪表)

> Gauges是一个最简单的计量，一般用来统计瞬时状态的数据信息。

Meters

> Meters用来度量某个时间段的平均处理次数（request per second），每1、5、15分钟的TPS。

Histograms

> Histograms主要使用来统计数据的分布情况，最大值、最小值、平均值、中位数，百分比（75%、90%、95%、98%、99%和99.9%）。

Timers

> Timers主要是用来统计某一块代码段的执行时间以及其分布情况，具体是基于Histograms和Meters来实现的。

## 名词解释

[吞吐率（Throughput）](https://ruby-china.org/topics/26221)

> 我们一般使用单位时间内服务器处理的请求数来描述其并发处理能力，称之为吞吐率。吞吐率特指 `Web` 服务器单位时间内处理的请求数。单位 `rpm`（Requests per minute）

## App.Metrics 配置（APP度量）

1，引用 `App.Metrics.AspNetCore.Mvc` 继而可以在 `ConfigureServices` 中进行依赖注册

```
var metrics = AppMetrics.CreateDefaultBuilder();

metrics.Configuration.Configure(x =>
{
    x.AddAppTag("APP"); x.AddEnvTag("Developer");
});

services.AddMetrics(metrics.Build());
```

2，配置数据存储（基于 `InfluxDb`）

```
var metrics = AppMetrics.CreateDefaultBuilder();

metrics.Report.ToInfluxDb(x =>
{
    x.InfluxDb.BaseUri = new Uri("时序数据HTTP地址");
    x.InfluxDb.Database = "数据库库名";
    x.InfluxDb.UserName = "数据库用户";
    x.InfluxDb.Password = "数据库密码";
    x.HttpPolicy.BackoffPeriod = TimeSpan.FromSeconds(30);
    x.HttpPolicy.FailuresBeforeBackoff = 5;
    x.HttpPolicy.Timeout = TimeSpan.FromSeconds(10);
    x.FlushInterval = TimeSpan.FromSeconds(5);
});

services.AddMetrics(metrics.Build());
```

3，注册报表调度服务（需要注册次中间件数据才能写入数据库）

```
services.AddMetricsReportScheduler();
```

4，添加开放指定的中间件，写入指定的信息

- `app.UseMetricsAllMiddleware()` 开启所有中间件
- `app.UseMetricsApdexTrackingMiddleware()` 开启应用程序性能数据收集中间件
- `app.UseMetricsErrorTrackingMiddleware()` 开启错误信息收集中间件
- `app.UseMetricsRequestTrackingMiddleware()` 开启HTTP请求请求信息收集中间件


## HiTSDB (阿里云时序数据库)

> OpenTSDB是一个分布式、可伸缩的时序数据库，支持高达每秒百万级的写入能力，支持毫秒级精度的数据存储，不需要降精度也可以永久保存数据。其优越的写性能和存储能力，得益于其底层依赖的HBase，HBase采用LSM树结构存储引擎加上分布式的架构，提供了优越的写入能力，底层依赖的完全水平扩展的HDFS提供了优越的存储能力。OpenTSDB对HBase深度依赖，并且根据HBase底层存储结构的特性，做了很多巧妙的优化。

### 数据模型

OpenTSDB采用按指标建模的方式，一个数据点会包含以下组成部分：

- metric：时序数据指标的名称，例如sys.cpu.user，stock.quote等。
- timestamp：秒级或毫秒级的Unix时间戳，代表该时间点的具体时间。
- tags：一个或多个标签，也就是描述主体的不同的维度。Tag由TagKey和TagValue组成，TagKey就是维度，TagValue就是该维度的值。
- value：该指标的值，目前只支持数值类型的值。


## App.Metrics Apdex

Apdex (Application Performance Index)  基于以下三个维度来衡量用户满意度

- Satisfied：Response time less than or equal to T seconds.
- Tolerating: Response time between T seconds and 4T seconds.
- Frustrating: Response time greater than 4T seconds.



## 阿里云日志

https://yq.aliyun.com/articles/227006