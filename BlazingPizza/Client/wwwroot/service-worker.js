﻿//Instalar el service worker
self.addEventListener("install", async event => {
    console.log('Instalando el service worker...')
    self.skipWaiting();
});

self.addEventListener("fetch", event => {
    //Podemos agregar aqui logica personalizada para controlar
    //si se pueden utilizar los datos en cache cuando la aplicacion 
    //se ejecute fuera de lineas

    return null;
})

self.addEventListener('push', event => {
    const payload = event.data.json();
    event.waitUntil(self.registration.showNotification('Blazing Pizza', {
        body: payload.message,
        icon: 'images/icon-512.png',
        vibrate: [100, 50, 100],
        data: { url: payload.url }
    }))
})

self.addEventListener("notificationclick", event => {
    event.notification.close();
    event.waitUntil(clients.openWindow(event.notification.data.url))
});