print("TimeScale Calculator")

def convert(seconds, ts): 
    seconds = seconds % ((24 * 3600) * ts)
    hour = seconds // (3600 * ts)
    seconds %= (3600 * ts)
    minutes = seconds // (60 * ts)
    seconds %= (60 * ts)
    
    return "%d:%02d" % (hour + 6, minutes)

while(True):
    i = 1
    mins = int(input("How many minutes: "))
    timeScale = float(input("Give me a timescale: "))
    while(i <= (60 * mins)):
        print(convert(i, timeScale))
        i += 1
