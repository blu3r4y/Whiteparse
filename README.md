| :warning: **This project is in a very early development stage.** |
|--------------|
| Whiteparse is not yet ready to be used in a productive environment. The API might change heavily, the documentation is not synced up and some functionality is not implemented. Feel free to try out Whiteparse or even extend it from here on, but do not use it for serious work yet. Whiteparse will eventually reach a stable state, with a stable API, releases and NuGet packages. If you are still interested, feel free to clone the repository and experiment with the work that is implemented already, but don't blame me if something breaks for you in the future. |

# Whiteparse

Whiteparse is a data parser and reverse templating engine, written in C# and optimized for parsing whitespace-delimited text to JSON objects.

## Tokens

The input text will be split into tokens.
Every sequence of whitespace character serves as a delimiter.

### Named Tokens

Named tokens parse the element at the specified location and create a key-value pair in the object.
Consider the following input to the parser.

    1 2.3 abc z

Using the grammer `$a $b $c $c`, Whiteparse will parse this object.

    {
        "a": 1,
        "b": 2.3,
        "c": "abc",
        "d": "z"
    }

#### Structured Identifiers

To build complex objects on the fly, use a structured identifier, delimited by dots `.`.
Consider the following input to the parser.

    313.39 316.98 -7.22 1.42

Using the grammer `$pos.x $pos.y $pos.vx $pos.vy`, Whiteparse will parse this object.

    {
        "pos": {
            "x": 313.39,
            "y": 316.98,
            "vx": -7.22,
            "vy": 1.42
        }
    }

### Typed Tokens

The target data type will be determined automatically.
However, one can specify the data type of variable names explicitly to `int`, `float`, `string` or `bool`.

Type specifiers need to be appended directly before the variable name.

    $[int]a $[float]b $[string]c $[bool]d

### Ignored Tokens

For every named token, a field will be parsed and stored.
If you do not want to include some fields in the final object, start the variable name with `?`.

    $a $b $?c

#### Garbage Token

If you do not need the token at any point at all (e.g. as a size identifier), you can use the garbage token.

    $_

### List Tokens

Tokens can be repeated and parsed to a list.

    5
    0 1 2 3 4

Using the grammer `$?size $value{$size}`, Whiteparse will parse this object.

    {
        "values": [0, 1, 2, 3, 4]
    }

#### Range specifiers

The range specifier `{}` can hold one of the following:
- `{42}` a positive natural number
- `{$n}` a parsed token, representing a positive natural number
- `{+}` one or more repetitions, as many as possible
- `{*}` zero or more repetitions, as many as possible
- `{*?}` zero or more repetitions, as few as possible, expanding as needed

#### List delimiters

By default, the list will be built upon whitespace delimiters.
However, the delimiter can also be specified explictily after a colon separator in the range specifier `{$n(',')}`.

    0,1,2,3,4,5,6,7,8,9
 
Using the grammer `$value{*(',')}`, Whiteparse will parse this object.

    {
        "values": [0, 1, 2, 3, 4, 5, 6, 7, 8, 9]
    }

You can also specify more than one list delimiter by appending more colons.

    0,1 2,3;4,5;6,7|8,9--10--11

Using the grammer `$value{*(',', ';', '|', ' ', '--')}`, Whiteparse will parse this object.

    {
        "values": [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11]
    }
    
Delimiters may be enclosed in single or double quotes, e.g. `$value{*(",", ';', '|', " ", "--")}` is equivalent to the above.

#### Inline lists

If you want to parse a bunch of tokens into a list, use the inline list syntax `[$a $b $c]`.

    0 1
    5 2 7 8

Using the grammer `[$a $b] [$a $b $c $d]`, Whiteparse will parse this object.

    {
        "AB": [0, 1],
        "ABCD": [5, 2, 7, 8]
    }

If you want to change the field name, use a variable.

### RegEx Tokens

Arbitrary regex expression can be matched by using the regex token notation `$()`.

    123hello

Using the grammer `$(\d+) $name`, Whiteparse will parse this object.

    {
        "unnamedRegex1": "123",
        "name": "hello"
    }

Nested parentheses are allowed within regex tokens.
If you want to change the field name, use a variable.
If you do not want to create a field at all, prepend the question mark to the capturing group `$?()`.

### NewLine Tokens

By default, line breaks are considered as whitespace.
If you would like to forcefully match a line break character, use the special special character `$.`.

    0
    1

Using the grammer `$a $. $b`, Whiteparse will parse this object.

    {
        "a": 0,
        "b": 1
    }

However, for the input `0 1` Whiteparse would fail to parse the object, since the line break is missing.

### Literal Tokens

For matching characters literally (including whitespace and line breaks), use the literal token `$""` or `$''`.

    1 2.3 garbage
    xyz 5

Using the grammer `$a $" 2.3 garbage" $$ $"xyz " $b`, Whiteparse will parse this object.

    {
        "a": 1,
        "b": 5
    }

The literal token will be glued to its left and right partners, i.e. no whitespace will be implicitly parsed here.
For that reason, you need to explicitly specify whitespace characters and line breaks.

When using the dobule quotes `$""`, double quotes need to be escaped `\"`.
Similary, with single quotes `$''`, those need to be escaped `\'`.

## Grammar and Semantics

### Whitespace

Whitespace and line breaks have no semantics in the grammar specification.
The grammar serves as a pattern for parsing the whitespace-delimited input text.

By default, all tokens and even inline text can be surrounded by arbitrary many whitespace.

    a b c
    a  b  c
    a
    b
    
    c a
    b
    
    c
    
Using the grammer

    $tuple{4}
    
    $tuple = [$a $b $c]

Whiteparse will parse this object.

    {
        "tuples": [
            [ "a", "b", "c" ],
            [ "a", "b", "c" ],
            [ "a", "b", "c" ],
            [ "a", "b", "c" ]
        ]
    }

#### What is a whitespace character?

Any character matching the regex `[ \t\r\n\f]` (spaces, tabs, carriage returns, line feeds and form feeds) plus all the unicode [whitespace characters](https://en.wikipedia.org/wiki/Whitespace_character#Unicode).
Internally, the [`Char.IsWhiteSpace` method](https://docs.microsoft.com/en-us/dotnet/api/system.char.iswhitespace?view=netframework-4.7.1#remarks) is used.

### Variables

You can specify additional variables in your grammar specification.

A variable automatically creates a new field in the parsed object, holding the left-hand side as its value.

    1 2

Using the grammer

    $point
    
    $$point = $x $y

Whiteparse will parse this object.

    {
        "point": {
            "x": 1,
            "y": 2
        }
    }

At the moment, you can not use variables multiple times within the same scope.

#### Typed Variables

Variables can optionally be typed to a C# type specified by its name.
The parser will then try to create an object of the specified type at runtime (with a matching constructor or reflection).
This feature is only available when parsing input to C# objects.

    1 2
    
Using the grammer

    $point
    
    $$[Point]point = $x $y
    
And the class definition

    public class Point
    {
        private int x;
        private int y;
    }

Whiteparse will parse a C# object, where the value represents a `Point` instance. 

    {
        "point": Point { x=1, y=2 }
    }

### Comments

Use the hashtag `#` for single-line comments.
All characters following the `#` in the current line will be ignored.

### Escape Characters

Use the backslash to escape special characters. The following characters need to be escaped in text:
- `\$` token prefix
- `\#` comment
- `\\` escape character itself
- `\(` + `\)` parentheses
- `\[` + `\]` square brackets 
- `\{` + `\}` curly braces

Within string literals, quotes are the only symbols which need to be escaped.
- `\"` in a double-quote string literal (e.g. the literal token `$""`)
- `\'` in a single-quote string literal (e.g. the literal token `$''`)

Regex tokens are not aware of escape characters.

### Multiline Statements

If you need to wrap the right-hand side of a variable declaration to multiple lines, use the `\` character to indicate that the current line is followed by another one.

    $variable = $a $b $c \
        $d $e $f \
        $g $h $i

### Semantic Requirements

- Variable names need to be globally unique
- Named tokens need to be unique within their scope (this implies that you can not use a variable multiple times within the same scope)
- Named tokens can shadow named tokens from an outer scope
- Referenced tokens (e.g. as a range specifier) must be parsed before their first occurrence
