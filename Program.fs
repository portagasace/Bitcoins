
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
    "ppatil1" + String(Array.init n (fun _ -> chars.[r.Next sz]))
    
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
 //GENERATE HASH KEY (STOP ON FINDING THE 1ST COIN)
let generateHash (n:int) = 
    let mutable leadingZeroCount = 1;
    let mutable randomString = ""
    let mutable  hashKey = ""
    while(leadingZeroCount <> n) do 
        randomString <- ranStr(30)
        hashKey <- randomString |> SHA256
        leadingZeroCount <- hashKey|>hashLeadingZerosCount
    printfn $"%s{randomString}"
    printfn $"%s{hashKey}"

//GENERATE HASH KEY (FIND COINS INFINITELY)
let generateHashh (n:int) = 
    let mutable leadingZeroCount = 1;
    while(true) do 
        let randomString = ranStr(30)
        let hashKey = randomString|>SHA256
        leadingZeroCount <- hashKey|>hashLeadingZerosCount
        if(leadingZeroCount = n) then 
            printfn $"%s{randomString}"
            printfn $"%s{hashKey}"


       

generateHashh(3)











    