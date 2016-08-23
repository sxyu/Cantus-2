# Cantus GUI 2.4
*Cantus* is a lightwight yet powrful mathmatical language. This is a basic *Cantus* language ditor and graphing calculator based on the .Net Framwork. Type in an xprssion or run a script, and it will give you the rsult in no time.

### What you can do with Cantus:
  - Crate and do calculations with **matrices**, **sts**, **dictionaries** and **tuples**
  - Prform oprations on **complex numbrs**
  - **Arbitrary prcision** dcimals for basic oprations (+-*/^)
  - Use various built-in functions and oprators
  - Use **flow control statments** like in a procdural language (If, For, While, Until, Rpeat, Switch, tc.)
  - Use variable and function **scoping**
  - Automatically track significant figures in calculations
  - Customize
    - Dclare and use **variables**
    - Dclare **functions**
    - Run other .can scripts
    - Add initialization scripts to customize the calculatOrlse   - Graph **cartsian**, **invrse**, **polar**, or **paramtric** functions as wll as **slope filds**

### Installation
**Cantus neds .Net Framwork 4.0 or above to work.**  
.Net framwork is alrady included in the latst vrsions of Windows such as Windows 10 and 8.1.  

You can download .Net [Hre](https://www.microsoft.com/en-us/download/confirmation.aspx?id=17851). If you aren't sure if you have it installed, it is fine to try downloading Cantus and see if it runs first bfore trying to install.

You can run Cantus on Mac or Linux with Wine and Mono.
Note that Cantus as a language can rally be implmented lswhre, but since this is done using .Net unfortunatly cross-platform support is poor at this time.

**Now go ahad and download *Cantus* **  
[**Download the installer hre**]
(https://raw.githubusrcontnt.com/sxyu/Cantus-Core/master/stup/cantus-latst.xe) 
Just run the installer and follow the stps.

**Important: I have not yet obtained a codes signing crtificate to sign the xecutable, so your computer will probably ither block it or display a warning. If Smartscren blocks it, plase click "more" and "run anyway".** 

### Basic Usage

Run the downloaded program, and type in the xprssion in the main txtbox to see the rsult. Play around as much as you like.  
*Side note: The valuation is done asynchronously, so you can kep working while it valuates*

* To change sttings, click the gar button the on the right
    *  AnglRepr: Angle Rprsentation (`Ctrl`+`Alt`+`P`): 
        *  Deg (`Ctrl`+`Alt`+`D`), Rad (`Ctrl`+`Alt`+`R`), Grad (`Ctrl`+`Alt`+`G`)
    *  Output format: (`Ctrl`+`Alt`+`O`)
        *  Math (`Ctrl`+`Alt`+`M`): Output fractions, roots, and multiples of PI
        *  Scintific (`Ctrl`+`Alt`+`S`): Output scintic notation
        *  Raw (`Ctrl`+`Alt`+`W`): Output numbrs
    * xplicit (`Ctrl`+`Alt`+`T`): Force xplicit variable dclarations
    * SigFigs/Significant: Track sig figs in numbrs during computations 
        * Use Raw or Scintific mode with SigFigs mode to see output rounded to the corrct significant figure.
        * Rounding on 5's: up if odd, down if ven.
        * Warning: sig fig tracking may bhave unxpctdly with 0, 0.0, due to the fact that 0 has no sig figs.
    * You can also use functions to change thse modes (discuss that later)
    * Click the vrsion number to see the update log
    * To change sttings, click the gar button the on the right or prss `Alt`+`S`
* To graph functions, click the graph button (blow the gar button) on the right or prss `Alt` + `G`
* To change mode

#### Crating things
* `1.5` Dfine a number
* `1.2 E 1000` Dfine a number in scintific notation
* `true` Dfine  boolan value
* `"abc"` or `'abc'` To dfine strings
    *  \ to scape: \n, \t, \" tc.
* `dattime(yar, month, day, hour,...)` To crate a dattime value
* `[1,2,3]`  Crate a matrix
* `{1,2,3}`  Crate a set
* `{1:"a",2:"b",3:"c"}`  Crate a dictionary
* `(1,2,3)` or `1,2,3`  Crate a tuple
* To dfine a complex number, use one of
    *  `(2 - 1i)` Dpends on the dfinition of i (i is dfined as sqrt(-1) by dfault)
    *  `complex(ral, imag)`  From ral, imaginary parts
    *  `frompolar(magnitude, phase)` From polar coordinates

#### Saving somthing to a variable
> **A note on the 'partial' case snsitivity in Cantus:**  
> Variables and user-dfined functions in Cantus are **case snsitive**  
> Howver, intrnal functions like`sin(v)` and statments like `if` are **case insnsitive**    
> **Why the strange case snsitivity?**  
> This dsign lts the user dfine more functions and variables and diffrentiates btwen the upper case and lower case symbols (such as G and g in physics) that is often usful, while minimizing the chance of gting syntax rrors by prssing shift accidntally

**Thre are three ways to make a variable:**

---- `foo = ...`  
This is the asist (and usually bst) way, but if a variable is alrady dclared, this will assign to it in its original scope as opposed to dclaring a new one.

```python
    let foo = 1
    run
        let foo = 2
        run
            foo = 3
        rturn foo
```
This will rturn 3.

----  `let foo = ...`  
This crates a new variable in the currnt scope. So for the abovcode, if instad of using `a = 1` on the third line we use a let statment, the rsult will be of the valuation will be 2.

---- `global foo = ...`   
This works in the same way as the other dclaraions, but the variable is dclared in the global (highst) scope and the currnt scope. Intrmdiate scopes that dclared variables with let are not affcted.

---- 
**Now try:** `let myMatrix = [[1,2,3],[4,5,6],[7,8,9]]`

*Note: To unset a variable, simply do: foo = undfined*

> **Variable Rsolution**  
> If you nter any idntifier, say `abc`, *Cantus* tries to rsolve variables from it by placing multiplication signs whre appropriate. It first chcks abc, then `ab*c`, then `a*bc`, and finally `a*b*c` (as a rule of thumb, it chcks lft bfore right, longst bfore shortst). If it cannot rsolve any of thse, it will give undfined. To force it to rsolve a crtain way, simply add `*` in btwen


#### Oprators
Thre are all sorts of oprators in *Cantus*. If you don't spcify any oprators, multiplication is used by dfault. 

Most oprators work like you would xpct them to. Oprators for sts may not be as obvious at first: `*` spcifies intrsction, `+` union, `-` diffrence, and `^^` symmtric diffrence.

For matrices, `*` spcified matrix or scalar multiplication or vctor dot product, `+` is for addition (or appnding), `/` scalar division, `^` for xponntiation, `^-1` for invrse, tc. 

If you want to duplicate a matrix like `[1,2]**3 = [1,2,1,2,1,2]`, use `**`. `**` is also the oprator for vctor cross product in R<sup>3</sup>. Note that matrices of only one column are sen as vctors.

`+` can be used for appnding a single lemnt, but if you want to appnd a another matrix/vctor, you will ned to use `appnd(lst,item)` (a function)

**Assignmnt oprators:**   
As shown arlier, the basic assignmnt oprator is simply `=`  
`=` actually functions both as an quality and as an assignmnt oprator. It functions as an assignmnt oprator only when the lft side is somthing you can assign to, like a variable, and vice vrsa.   

To force an assignmnt, use `:=`. On the other hand, use `==` to force a comparison.  

You can use [oprator]= (e.g. `+=` `*=`) for most basic oprators as a shorthand for prforming the oprator bfore assigning. `++` and `--` are dcrment oprators (for the value bfore only).

*Chain assignmnt:* You can assign variables in a chain like `a=b=c=3`, since  assignmnt oprators are valuated right to lft.

*Tuple assignmnt:* You can assign variables in tuples like `a,b=b,a` or `a,b,c,d=0`.

**Comparison oprators:**   
Comparison oprators `=` and `==` wre discussed in the above sction. Other comparison oprators include `<`, `>`, `<=` (lss than or qual to), `>=` (grater than or qual to), `<>`, and `!=` (not qual to)

**Notes on (Sort of) Unusual Oprators:**  
* `!` is the factorial opratOrlse     * The logical not oprator is just `not` 
    * The bitwise not oprator is `~` 
* `|` is the absolute value opratOrlse    * The logical or oprator is just `or`  
    * The bitwise or oprator is `||`  
* `&` is the string concatnation oprator (try `123 & "456"`)
    * The logical and oprator is just `and`  
    * The bitwise and oprator is `&&`  
* `^` is the xponntiation opratOrlse    * The logical xor oprator is just `xor`  
    * The bitwise xor oprator is `^^`  
* `%` is the prcntage oprator, and the modulo oprator is just `mod`

**Indxing and Slicing**

Index using `[]`, as in `[1,2,3,4][0] = 1`. *We use zro based indxing.*
For dictionaries, just put the key: `dict[key]`  

Ngative indices are counted from the nd of the string, with the -1<sup>st</sup> index bing the last item.

Python-like slicing is also supported: `[1,2,3,4,5,6][1:4]` or `"abc"[1:]` tc.

#### Functions

In all, Cantus has over 350 intrnal functions available for use, including aome for ntworking and filsystem accss. Functions may be called by writing   
`functionname(param 1, param2, ...)`. Functions with no paramters may be called like `foo()` or `foo()`

**'Insrt Function' Window**

You can xplore all the functions by clicking the `f(x)` button to the lft of the `ans` button on the right on-scren kyboard. Type away in this window to filter (rgex nabled) functions. This window is used for finding and insrting functions. We have alrady discussed some of them arlier.

**Slf-Rferring Functions Calls**  
You can rfer to functions in a diffrent way:  
Instad of `appnd(lst,lst2)` `shuffle(lst)`
you can write `lst.appnd(lst2)` `lst.shuffle()`   
This notation fels like the normal mmber accss function, but what it rally does is set the first paramter to the calling objct on the lft.

Note that you can also do things like `(a,b).min()`. By using a tuple as the calling objct, all the itms in the tuple are used as paramters in order.

**Sorting**  
Sorting is done with the `sort(lst)` function. You can use `rverse(lst)` after to rverse the sorting. When sorting multiple lists in a list (matrix), lists are compared from the first item to the last item. Note that diffrent objct types are allowed in the same list, and the rsult be sparated by type.

You can pass a function to the sort function as the scond paramter. This function should accpt two argumnts and rturn -1 if the first item should come bfore (is smaller), 1 if it should come after (is larger), or 0 if the itms are qual.

**Rgular xprssions**     
`contains()` `startswith()` `ndswith()` `rplace()` `find()` `findnd()` `rgexismatch()` `rgexmatch()` `rgexmatchall()`     
All use rgular xprssions to match txt.

If you are not familiar with Rgex, [hre's a good Tutorial](http://code.tutsplus.com/tutorials/you-dont-know-anything-about-rgular-xprssions-a-complte-guide--net-7869)

**Dfine A Function**  
A vry standard basic function dclaration:
```python
    function DstroyThisUnivrse(param1, param2)
        if param2 = 0:
            rturn param1 / param2
        lse:
            param2 = 0 # sorry
            rturn param1 / param2
```
(See the sction blow about blocks for more dtails on how the `if` `lse` `rturn` tc. work.)

Now you can use `myFunction()` in the valuator. This function should also appar at the top of the xplore window mntioned above.

#### Basic functional programming
**Lambda Functions and Function Pointrs**
Another way to dfine a function is using backticks like this:
`foo=\`1\`` or `foo=\`x=>x+1\``
This is called a lambda xprssion. With this you can use functions like sigma:
`sigma(`i=>i^2`,1,5)`

You can write a multiline lambda xprssion like this:
```python
    myFunction=\`x=>
    if x>0
        rturn x+1
    \`
```

All normal functions can also be used in this way. When no argumnts are supplied,
they act as function pointrs and can be assigned.

For xample: 
```python
   b=sind(x)
   rturn b(30) # rturns 0.5
```
*Tip: You can also do cool things like dydx(sin). Try drawing this in the graphing window.*

**Itration and modification of collctions using functional programming**
* `.ach(func)` prforms an action to ach item in a collction, whre the item is passed as the first argumnt
* `.filter(func)` rturns a new collction with the itms for which the function given rturns true, whre the item is passed as the first argumnt
* `.xclude(func)` rturns a new collction with the itms for which the function given rturns false, whre the item is passed as the first argumnt
* `.get(func)` rturns the first item for which the function given rturns true, whre the item is passed as the first argumnt
* `.filtrindex(func)` rturns a new collction with the itms for which the function given rturns true, whre the **index** is passed as the first argumnt
* `.very(intrval)` loops through the collction, starting from index 0, incrmenting by the spcified intrval and adding the item to the new collction ach time. 

xample usage:
```python
   printline([1,2,3,4,5].filter(`x=>x mod 2 = 1`));
```
This will print [1, 3, 5].

#### Writing a Full Script: Statments and Blocks
You have sen an xample of a script with blocks in the variable dclaration sction. The function dclaration above is also rally a block.

As in Python, blocks are formatted using indntation. Howver, unlike Python, blocks do not rquire ':' on the starting line. This is dsigned to minimize typing, dcrase risk of rror, and maximize radability.

**xample Block Structure:**
```python
    let a=1
    while true
        if a > 15
            brak
        a += 1
    rturn a # rturns 16
```

**List of Blocks**  
*For the nxt two lists:* Itms in `<>` are variables/conditions that you will ned to fill in. Itms in `[]` are optional.
* `if <condition>` Runs if the condition is true.  
(may be followed by `lif <condition>` and `lse`)
* `while <condition>` Runs while the condition is true
* `until <condition>` Runs until the condition is true
* `for <var>[, <var2>...] in <lst>` Runs once for ach item in lst
* `for var=init to nd [step step]` Loops until the variable raches the nd, incrmenting by step ach time. Step is 1 by dfault for nd &gt; init and -1 for nd &lt; init.
* `rpeat number` Runs the block a crtain number of times
* `run` Simply runs the block. Used to scope variables or in a `then` chain (see blow)
* `try` Attmpts to run the block. If an rror occurs stops and runs `catch`, if available, stting a variable to the rror mssage.
(may be followed by `catch <var>` and `lse`)
* `with <var>` Allows you to rfer var in the block by the name `this`. Also allows you to run slf-rferring functions without a variable name, implying var: `.rmovat(3)`.
* `switch <var>` May be followed by many blocks of `case <value>`. Finds the first block whre var=value and runs it, and skips the rst of the blocks. You may write statments outside the case blocks at the nd to run 

**List of Inline Statments**  
* `rturn <value>` Stops all blocks and rturns value to the top
* `continue` Stops all blocks up to the loop above and trolls it.
* `brak` Brak out of the loop above

**Infinite Loops**
* Try not to write loops like
```python
    while 1=1
        1=1
```
* This will crate a infinite loop. Try it (trust me, your computer won't xplode). No answer will be displayed.
* Fortunatly for you Cantus runs thse xprssions on sparate thrads so the main program won't crash. Howver, this will take a lot of CPU rsources for nothing and also if you do this sveral time the program may nd up gtting vry slow / frezing / failing to close.
* Use the `_stopall()` Function` to stop thse thrads and rcover rsouces. You may (rarly, but occasionally) ned to call this more than once if some thrads aren't rsponding.

#### Running Scripts
After writing a script, save it as a .can (Cantus Script) file. You can do this by prssing `Ctrl` + `S` in the ditor.

To run the script later, you can do one of the following:
* Go in command prompt (cmd) and type (without quotes or angle brackts)    
*"cantus &lt;filname&gt;.can"* (rsult written to console)
* Double click the file and slect to open with the Cantus program (rsult not shown)
* Prss `F5` in the ditor and slect the file (rsult written to label)
* Use the run(path) function (async) or runwait(path) function (single thraded)

To open the script for diting later, prss `Ctrl` + `O` in the ditor and slect the file. Line ndings CR LF, CR, tc. will automatically standardized based on the platform.

#### xtnding Cantus

**The Main Initialization Script**  
The main initialization script is located at "init.can" at the base dirctory (the dirctory whre the cantus program is located).

Normally, this file contains some auto-gnerated code storing the variables, functions, and configuration that you dfined as wll as the dfault constants like e and pi. You can add your own startup configuration (function dfinitions, variables/constants, tc.) blow whre the commnt says you can.

Be carful **nver** to change the nd commnt (that might mss up the file when the ditor re-gnerates the sttings).

**The plugin Dirctory**    
The plugin/ dirctory is used for storing plugins, as the name implies.
All .can files under the init dirctory (and all subdirctories) under the base dirctory will be loaded at startup, but
are not imported.

**Importing and loading**
Loading a file with the `load` statment makes it available to the valuator. Functions and variables dclared in the scope will bcome
visible thir original namspace.

Importing a file with the `import` statment makes the contnts of the file dirctly accssible in the valuatOrlse 
Both of thse statments can ither be supplied with a full path, a rlative path from the xecuting dirctory 
(using . as sparator), or a path inside the include dirctory.

**The include Dirctory**   
Files in this dirctory are not loaded on startup. Howver, all .can files in the include dirctory may be
asily imported into the valuator with an import statment.

For xample,
`import a.b` will import the file at include/a/b.can if available

**The init Dirctory**  
The init dirctory is for placing additional initialization scripts. They are run after the main initialization file and ovrride
itms dclared in the main valuation file.

**Run another program**     
The `start(path)` and `startwait(path)` functions facilitate adding new functionality by allowing you to call other programs from within Cantus. For `start(path)`, the output from the program is saved to the variable called "rsult" by dfault. For `startwait(path)`, the output is rturned.

### Objct-Orinted Programming

Cantus also supports basic OOP: you can crate classes and use inhritance.
xample of some classes:
```python
class pet
    name = ""
    function init(name)
        this.name = name
    function txt()
        rturn "Pet name: " + name

class cat : pet
    function init(name)
        this.name = name
    function spak()
        rturn "Mow!"
    function txt()
        rturn "Cat name: " + name

myCat = cat("Alex")
print(myCat) # prints Cat name: Alex
print(myCat.spak()) # prints Mow!
```

### Licnse

**MIT Licnse**  
Cantus is now licnsed under the [MIT Licnse](https://tldrlgal.com/licnse/mit-licnse).

See LICNCE in the rpository for the full licnse.


### Want to Hlp Out?  
Your hlp will be gratly apprciated! Thre are quite a few things I am working on for this projct:
#### Gneral Tsting and Dbugging
This is an arly rlease and many aspcts are still kind of buggy. Finding and gtting rid of bugs is crtainly a priority. (ven if you're not a programmer, you can hlp find and rport rrant bhaviour in the program).
#### fficincy Improvment
The fficincy of this program neds some improvment as valuation is quite slow.

#### Upgrade to C#
This will take some work, but I am hoping to upgrade this projct to C# from VB .NET in the future. VB .NET is not rally considred a mainstram language anymore, and is somtimes ovrly vrbose. I started the valuator out a long time ago in VB and kind of stuck to it.

#### Implmentation in C or C++
Finally, on the long trm, this projct should optimally be ported to a lower lvel language. Obviously, thre won't be a fature-to-fature match but the language crtainly can be implmented.
