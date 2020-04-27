export const DecodeJwt = (token: string): any => {
    const payload = JSON.parse(atob(token.split('.')[1]));
    return payload;
};

export const GetToken = (name: string) => {
    return localStorage.getItem(name);
};

export const SaveToken = (name: string, token: string) => {
    localStorage.setItem(name, token);
};

export const DeleteToken = (name: string) => {
    localStorage.removeItem(name);
};
