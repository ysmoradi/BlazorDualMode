function setCookie(name: string, value: string, seconds: number): void {
    const date = new Date();
    date.setSeconds(date.getSeconds() + seconds);
    const expires = 'expires=' + date.toUTCString();
    document.cookie = name + '=' + value + '; ' + expires + '; path=/';
}

function getCookie(name: string): string {
    const cookies = document.cookie.split(';');
    for (let i = 0; i < cookies.length; i++) {
        const cookie = cookies[i].split('=');
        if (trim(cookie[0]) == escape(name)) {
            return unescape(trim(cookie[1]));
        }
    }
    return null;
}

function trim(value: string): string {
    return value.replace(/^\s+|\s+$/g, '');
};

function removeCookie(name: string): void {
    document.cookie = name + '=; Max-Age=0';
}