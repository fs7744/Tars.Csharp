# Tars.Csharp

Tars.Csharp 是 https://github.com/Tencent/Tars 的 dotnet 版本

项目基于 netstandard2.0 标准 

（ps:其实理论1.3就够了, 但为了偷懒，所以没有检查所有api在1.3的支持情况）

## PS

目前项目处于刚起步阶段，暂未完善

初期目标如下：
- [X] Tars rpc server and client simple Demo
- [X] Tars rpc 协议解析完善 (大致整理了，但未作完整测试，并且代码应该可以进行优化)
- [X] Tars rpc 同步 / 异步(Task/ValueTask) / 单向 (TarsOnewayAttribute) 调用完善
- [ ] 函数重载支持
- [ ] Servant 定位路由
