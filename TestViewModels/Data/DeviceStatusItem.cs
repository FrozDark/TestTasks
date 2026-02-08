using System;
using System.Collections.Generic;
using System.Text;
using TestModels;

namespace TestViewModels.Data;

public sealed partial class DeviceStatusItem(DeviceStatus? status, string display)
{
    public DeviceStatus? Status { get; } = status;
    public string Display { get; } = display;
}
