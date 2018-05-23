using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Threading;

[Serializable]
public abstract class Protocol : Object 
{
    public Protocol()
    {

    }

    public virtual Protocol Clone()
    {
        DEBUG.Error("This Function is Deprecated!");
        return null;
    }

    public abstract byte[] toBytes();

    public abstract int parseBytes(byte[] bytes, int begin);
}
