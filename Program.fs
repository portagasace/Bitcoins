
open System
open System.Security.Cryptography
open Akka.Actor
open Akka.Configuration
open Akka.FSharp
open Akka.TestKit

// #Simplify Actor
// There is a simpler way to define an Actor

let ranStr n = 
    let r = Random()
    let chars = Array.concat([[|'a' .. 'z'|];[|'A' .. 'Z'|];[|'0' .. '9'|]])
    let sz = Array.length chars in
    String(Array.init n (fun _ -> chars.[r.Next sz]))
    


let SHA256 (text:string) =
   
    use sha256Hash = SHA256Managed.Create()
    sha256Hash.ComputeHash(System.Text.Encoding.ASCII.GetBytes(text))

let string1 = "aprakash" + ranStr(30)
printfn $"Random String is: %s{string1}"

let hash = System.Text.Encoding.ASCII.GetString(SHA256(string1))
printfn $"Hash is: %s{hash}"



let n = System.Console.ReadLine()










    