#include <stdio.h>
#include <string.h>

char *fill_source(char *dst)
{
    char *p = dst;
    for (char *a = "ab"; *a != 0; a++)
    for (char *b = "abcdef"; *b != 0; b++)
    for (char *c = "abcdef"; *c != 0; c++)
    for (char *d = "abcdef"; *d != 0; d++)
    {
        for (int i = 0; i < 8; i++)
        {
            *p++ = *a;
            *p++ = *b;
            *p++ = *c;
            *p++ = *d;
        }
        *p++ = ' ';
    }
    *--p = '\0';
    return p;
}

char *replace(char *dst, char* src, char* transform(char* dst, char* src, int len))
{
    char *s = src;
    char *d = dst;
    while (*s != '\0') {
        char* f = strchr(s, ' ');
        char* end = f == NULL ? s + strlen(s) : f;
        d = transform(d, s, end - f);
        *d++ = ' ';
    }
    *--d = '\0';
}

char *

int main(int argc, char* argv[])
{
    printf("Size of char: %d\n", (int)sizeof(char));
    printf("Size of char[2]: %d\n", (int)sizeof(char[2]));
    printf("Size of abc: %d\n", (int)sizeof("abc"));

    char src[1024 * 100];
    char* p = fill_source(&src[0]);
    printf("%s\n", src);
    printf("%lu\n", strlen(src));
}
