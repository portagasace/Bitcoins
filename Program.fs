
open System
open System.Security.Cryptography
open Akka.Actor
open Akka.Configuration
open Akka.FSharp
open Akka.TestKit
open Microsoft.FSharp.Core


//GENERATE RANDOM STRING
let ranStr n = 
    let r = Random()
    let chars = Array.concat([[|'a' .. 'z'|];[|'A' .. 'Z'|];[|'0' .. '9'|]])
    let sz = Array.length chars in
    String(Array.init n (fun _ -> chars.[r.Next sz]))
    
 //GENERATE HASH KEY FROM RANDOM STRING
let SHA256 (text:string) =
    System.Text.Encoding.ASCII.GetBytes(text)
    |>(new SHA256Managed()).ComputeHash 
    |> Array.map (fun (x : byte) -> System.String.Format("{0:X2}", x))
    |> String.concat System.String.Empty

//COUNT NO OF LEADING 0'S FOR HASH KEY
let hashLeadingZerosCount (hashkey :string) = 
    let mutable count =0
    let mutable flag = 0
    for elem in hashkey do
        if elem = '0' && flag <> 1 then
            count <- count+1
        elif elem <> '0'  then
             flag <- 1
    count    
 
//GENERATE HASH KEY UNTILL LEADING ZEROS DONT BECOME N

    
        
let hashkey = "0000003003F0DA82F464864184E5F578E2B529447FC1F2D57A4154720FD698EFD5CA40496"
printfn $"Leading zeros are : %i{hashLeadingZerosCount(hashkey)}"












    