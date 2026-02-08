using System;
using System.Collections.Generic;
using System.Text;

namespace TestModels;

public sealed partial class Monitor : OutputDevice
{
    public override string Type => nameof(Monitor);
}
