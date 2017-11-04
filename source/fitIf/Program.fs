open System.Net
open System.Net.Sockets
open System.Text

[<EntryPoint>]
let main argv = 
    let listener = new TcpListener(IPAddress.Parse("127.0.0.1") ,8081)
    listener.Start()
    printfn "listening ..."
    let client = listener.AcceptTcpClient()
    printfn "accepted ..."
    let stream = client.GetStream()
    let bytes = Array.create<byte> 256 0uy
    let length = stream.Read(bytes, 0, 256)
    let line = Encoding.ASCII.GetString(bytes, 0, length)
    printfn "%s" line
    client.Close()
    listener.Stop()
    0
