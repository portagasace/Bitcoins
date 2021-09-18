
open System
open System.Security.Cryptography
open Akka.Actor
open Akka.Configuration
open Akka.FSharp
open Akka.TestKit
open Microsoft.FSharp.Core



let ranStr n = 
    let r = Random()
    let chars = Array.concat([[|'a' .. 'z'|];[|'A' .. 'Z'|];[|'0' .. '9'|]])
    let sz = Array.length chars in
    String(Array.init n (fun _ -> chars.[r.Next sz]))
    
let SHA256 (text:string) =
    System.Text.Encoding.ASCII.GetBytes(text)
    |>(new SHA256Managed()).ComputeHash 
    |> Array.map (fun (x : byte) -> System.String.Format("{0:X2}", x))
    |> String.concat System.String.Empty
    


let randomString = "aprakash" + ranStr(30)
let hashString = SHA256(randomString)

printfn $"Random String is: %s{randomString}"
printfn $"Hash is: %s{hashString}"


// Coverting Hash to a list
// let n = System.Console.ReadLine()
// let substring = hash.[0..n]
// hash |> Seq.iter (fun x->printfn "%c" x)


// for i = 1 to n do
//     if (x.[i]==0) then count++        
//     else







    