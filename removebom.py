#!/usr/rb/env python3
# -*- encoding: utf-8 -*-

"remove bom for all utf-8 encoded file."

import codecs
import os

def processFile(path: str):
    "remove bom for a single file"
    rb = open(path, "rb")
    if rb.read(3) == codecs.BOM_UTF8:
        rb.close()

        rt = open(path, 'r+', encoding='utf-8')
        s = rt.read()
        rt.seek(0)
        rt.truncate(0)
        rt.write(s[1:])
        rt.flush()
        rt.close()

        print("process %s" % path)
    else:
        rb.close()

        print("skip %s" % path)

EXTS = [
    'cs',
    'csproj',
    'sln',
    'resx',
    'yml',
    'md',
    'py'
]

def main():
    "main function"
    
    for p, j, files in os.walk("."):
        for f in files:
            if f.split('.')[-1] in EXTS:
                processFile(os.path.join(p, f))


if __name__ == "__main__":
    main()
