Once in a while you read another dunning-kruger нахрюк about "language X is 
faster than Y a priori, no matter what", coming in a nice company of low-level 
jokes implying that your unwilling to agree is just a consequence of being 
stupid, "go tell that to <highly pointed stackoverfow answer>" or another 
humiliating variation of authority-based argument. Obviously, the latter means 
that person just have read something on the internet many times and blindly 
believes it while having no real proofs.

Well, my dear, since the context of talk was that there is no sense of choosing 
a specific language for a simple proxy and your comments were sarcastic "ya-ya, 
python can be compared to c#, fat chance", here's a primitive python file 
server that not only performs on same magnitude as C# - that wouldn't be 
surprising - but even outbeats it, despite having nearly 1-to-1 matching code.  

C# version:
  
```
Time per request:       4056.288 [ms] (mean)
Transfer rate:          4136100.30 [Kbytes/sec] received
```
  
Python version:
  
```
Time per request:       3181.594 [ms] (mean)
Transfer rate:          5273211.25 [Kbytes/sec] received
```

Technically, it's not the runtime that is at play here, but rather language, or 
which capabilities it provides (and also I'm running this on Linux, that's 
important). And yes, I won't lie to you, it's a bit of configuration play (but 
that's expected since one has to compensate python's extra CPU burn with 
performance quirks, it's basically creating a satisfying proportion).

Can you spot (and explain) the key point here? You can get your hints analyzing 
why there are some pieces for code that are actually not necessary to throw 
another kid off from his stupidity throne. Also, if you know what's going on, 
you probably know that current python code may be made even faster by directly 
calling necessary API (reducing number of python stack frames), but that would 
ruin the whole seek-that-quirk game.

### Ряяяяя, I don't believe ya!1111

Test it yourself.

```
python3 main.py &
ab -c 1 -n 100 http://localhost:48080/
```

then

```
dotnet publish -c Release
bin/Release/netcoreapp5.0/publish/Program &
ab -c 1 -n 100 http://localhost:48081/
```