﻿module CoinMiner


open Akka
open Akka.FSharp
open Akka.Cluster
open System
open Cutils
open Configs

[<EntryPoint>]
let main argv =

    let zeroCount = (argv.[1] |> int)
    let nodeType = argv.[0]
    let seedHostName = if nodeType<>"singleNode" then argv.[2] else ""
    let port = if nodeType<>"singleNode" then argv.[3] else ""
    let hostName = if nodeType="client" then argv.[4] else ""
    let systemName = if nodeType = "singleNode" then "coin-miner" else "coin-mining-cluster"
    let nodeName = sprintf "cluster-node-%s" Environment.MachineName
    
    let zeroCount = (argv.[1] |> int)
    let nodeType = argv.[0]
    let seedHostName = if nodeType<>"singleNode" then argv.[2] else ""
    let port = if nodeType<>"singleNode" then argv.[3] else ""
    let hostName = if nodeType="client" then argv.[4] else ""
    let systemName = if nodeType = "singleNode" then "coin-miner" else "coin-mining-cluster"
    let nodeName = sprintf "cluster-node-%s" Environment.MachineName

    let minerName = 
        if nodeType = "singleNode" then "akka://" + systemName + "/user/minerProvider" else "akka.tcp://" + systemName +  "@" + seedHostName + ":" + port + "/user/minerProvider"

    let hashGen (mailBox: Actor<MessageType>) = 
        let rec loop() = actor {
            let! message = mailBox.Receive();
            match message with
            | GenerateRandomString ->
                let randomStr = ranStr 6
                select minerName mailBox.Context.System <! {Type=Mine; Value=randomStr; Nonce=0}
                mailBox.Self <! GenerateRandomString
            | _ ->
                printfn "%s : The message is Invalid" (mailBox.Self.Path.ToStringWithAddress())
            return! loop()
        }
        loop()
    
    let mineWork (mailbox: Actor<Message>) =
        let rec loop() = actor {
            let! message = mailbox.Receive ()
            match message.Type with
            | Mine -> 
                let input = if message.Nonce > 0 then (message.Value + message.Nonce.ToString()) else message.Value
                let hash = SHA256 input
                if hashLeadingZerosCount hash = zeroCount then
                    printCoin zeroCount input hash (mailbox.Self.Path.ToStringWithAddress())
                elif message.Nonce < Int32.MaxValue then
                    message.Nonce <- (message.Nonce+1)
                    select minerName mailbox.Context.System <! message
            | _ ->
                printfn " Invalid message received" 
            return! loop ()
        }
        loop()
     
    if nodeType = "seed" then  
        printfn "Starting the seed node"
        let seedSystem = seedAkkaConfig seedHostName port |> System.create systemName

        let boss =
            spawn seedSystem nodeName (fun (mailBox: Actor<ClusterEvent.IClusterDomainEvent>) -> 
                let cluster = Cluster.Get seedSystem
                cluster.Subscribe(mailBox.Self, [| typeof<ClusterEvent.IClusterDomainEvent> |])
                mailBox.Defer(fun () -> cluster.Unsubscribe(mailBox.Self))
                let rec loop () = 
                    actor {
                        let! message = mailBox.Receive()                        
                        match message with
                        | :? ClusterEvent.MemberJoined as event -> 
                            printfn "Remote node %s joined the Cluster at %O" event.Member.Address.Host DateTime.Now
                        | :? ClusterEvent.MemberLeft as event -> 
                            printfn "Remote Node %s left the Cluster at %O" event.Member.Address.Host DateTime.Now
                        | other -> 
                            printfn "Cluster task received %O at %O" other DateTime.Now
        
                        return! loop()
                    }
                loop())

        let cluster = Cluster.Get seedSystem
        cluster.RegisterOnMemberUp (fun () -> 
            spawnOpt seedSystem "minerProvider" mineWork [ Router(Akka.Routing.FromConfig.Instance) ] |> ignore
            let genRouter = spawnOpt seedSystem "generatorProvider" hashGen [ Router(Akka.Routing.FromConfig.Instance) ]
            genRouter <! GenerateRandomString
            initStatParams()
        )

        0 |> ignore

    elif nodeType = "client" then
        printfn "Starting the client node"
        let actorSystemClient = clientAkkaConfig hostName port seedHostName |> System.create systemName
        initStatParams()
        
        let clientListenerRef =  
            spawn actorSystemClient "clientListener"
            <| fun mailbox ->
                let cluster = Cluster.Get (mailbox.Context.System)
                cluster.Subscribe (mailbox.Self, [| typeof<ClusterEvent.IMemberEvent>|])
                mailbox.Defer <| fun () -> cluster.Unsubscribe (mailbox.Self)
                printfn "An actor node is created with [%A] with roles [%s]" cluster.SelfAddress (String.Join(",", cluster.SelfRoles))
                let rec loop () = actor {
                    let! (msg: obj) = mailbox.Receive ()
                    match msg with
                    | :? ClusterEvent.MemberRemoved as actor -> 
                            printfn "The actor was removed %A" msg
                    | :? ClusterEvent.IMemberEvent           -> 
                            printfn "Task recieved from cluster %A" msg
                    | _ -> 
                            printfn "Message recieved: %A" msg
                    return! loop () 
                }
                loop ()
        0 |> ignore

    elif nodeType = "singleNode" then
        printfn "Starting the single node"
        let system = System.create systemName <| singleNodeConfig
        spawnOpt system "minerProvider" mineWork [ Router(Akka.Routing.FromConfig.Instance) ] |> ignore
        let genRouter = spawnOpt system "generatorProvider" hashGen [ Router(Akka.Routing.FromConfig.Instance) ]
        genRouter <! GenerateRandomString
        initStatParams()

    readInput()

    0 // Return an integer exit code
