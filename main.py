import sys
import socket
import time
import tempfile

BUFFER_SIZE_SHIFT = max(0, int(sys.argv[1])) if len(sys.argv) > 1 else 6
BUFFER_SIZE = 8192 << BUFFER_SIZE_SHIFT
ITERATIONS = 1024 * 32


def timestamp():
    return int(round(time.time() * 1000))


print('Buffer size: %d (shift: %d)' % (BUFFER_SIZE, BUFFER_SIZE_SHIFT))
print('Iterations: %d' % ITERATIONS)


with tempfile.TemporaryFile() as source:
    source.write(bytearray(BUFFER_SIZE))
    with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as listener:
        listener.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
        listener.bind(('127.0.0.1', 48080))
        listener.listen(1)
        print('Listening for incoming connections')

        while True:
            (client, address) = listener.accept()
            print('Accepted client connection')
            client.recv(BUFFER_SIZE)

            client.send(b"HTTP/1.1 200 OK\r\n\r\n")
            snapshot = timestamp()
            for i in range(ITERATIONS):
                client.sendfile(source, offset=0, count=BUFFER_SIZE)
                source.seek(0, 0)
            print('File proxying took %d milliseconds' % (timestamp() - snapshot))

            print('Done serving client connection')
            client.shutdown(socket.SHUT_RDWR)
            client.close()
