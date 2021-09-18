
open System
open System.Security.Cryptography
open Akka.Actor
open Akka.Configuration
open Akka.FSharp
open Akka.TestKit
open Microsoft.FSharp.Core


// Generating a Random String
let ranStr n = 
    let r = Random()
    let chars = Array.concat([[|'a' .. 'z'|];[|'A' .. 'Z'|];[|'0' .. '9'|]])
    let sz = Array.length chars in
    String(Array.init n (fun _ -> chars.[r.Next sz]))
    

// Computing Hash
let SHA256 (text:string) =
   
    use sha256Hash = SHA256Managed.Create()
    sha256Hash.ComputeHash(System.Text.Encoding.ASCII.GetBytes(text))


let string1 = "aprakash" + ranStr(30)
printfn $"Random String is: %s{string1}"

let hash = System.Text.Encoding.ASCII.GetString(SHA256(string1))
printfn $"Hash is: %s{hash}"

// Coverting Hash to a list
let n = System.Console.ReadLine()
let substring = hash.[0..n]
hash |> Seq.iter (fun x->printfn "%c" x)


for i = 1 to n do
    if (x.[i]==0) then count++        
    else

//

//while (n < 20) do










    