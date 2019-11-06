from PIL import Image
import os
from glob import glob
from sys import argv

## Structure
# 4 bytes - count of images
#
# Image structure
# 2 bytes: width
# 2 bytes: height
# 2 bytes: name length
# n bytes: name
# 4 bytes: data length
# n bytes: data
#
# All images stored in 8.8.8.8 RGBa format

def int8tobytes(value):
    return [value & 0xFF]

def int16tobytes(value):
    return [
        (value >> 8) & 0xFF,
        value & 0xFF
    ]

def int32tobytes(value):
    return [
        (value >> 24) & 0xFF,
        (value >> 16) & 0xFF,
        (value >> 8) & 0xFF,
        value & 0xFF,
    ]    

if __name__ == "__main__":

    out = "out.pack"
    filter = "*.png"
    archive = "none"

    if "--help" in argv:
        print("Usage: python pack.py --out --filter --archive")
        print("     --out - Output filename. Default is: \"{}\"".format(out))
        print("     --filter - Image searching filter. Default is: \"{}\"".format(filter))
        print("     --archive - Archiving method. Avaiable: zip, none. Default is: \"{}\"".format(archive))
        exit(0)

    for word in argv:
        if word.startswith("--out"):
            out = word.split("=")[1]
        if word.startswith("--filter"):
            filter = word.split("=")[1]
        if word.startswith("--archive"):
            archive = word.split("=")[1]

    print("Output filename: \"{}\"".format(out))
    print("Filter: \"{}\"".format(filter))
    print("Archive type: \"{}\"".format(archive))

    if os.path.isfile(out):
        answer = input("Output file \"{}\" already exists. Replace it? [Y/n]: ".format(out))
        if answer.lower() != "y":
            exit(0)
    
    files = glob(filter)
    print("Found {} files".format(len(files)))

    with open(out, "wb") as file:
        file.write(bytes(int32tobytes(len(files))))

        for imName in files:
            im = Image.open(imName)
            width, height = im.size

            file.write(bytes(int16tobytes(width)))
            file.write(bytes(int16tobytes(height)))

            nameBytes = bytes(imName, encoding="utf-8")
            file.write(bytes(int16tobytes(len(nameBytes))))
            file.write(nameBytes)

            im = im.convert("RGBA")
            pixels = []
            for y in range(0, height):
                for x in range(0, width):
                    color = im.getpixel((x, y))
                    pixels += int8tobytes(color[0])
                    pixels += int8tobytes(color[1])
                    pixels += int8tobytes(color[2])
                    pixels += int8tobytes(color[3])
            
            print("Image[{}x{}. Pixels: {}]".format(width, height, len(pixels)))
            file.write(bytes(int32tobytes(len(pixels))))
            file.write(bytes(pixels))
    
    pass