
let tss = document.getElementsByClassName('date-time');
for (let i = 0; i < tss.length; i++) {
    let content = tss[i].textContent.trim();
    if (content.length > 10) {
        tss[i].textContent = new Intl.DateTimeFormat('en-IN',
            { dateStyle: 'short', timeStyle: 'medium', hourCycle: 'h24' })
            .format(new Date(content));
    }
}

let tsl = document.getElementsByClassName('date-time-long');
for (let i = 0; i < tsl.length; i++) {
    let content = tsl[i].textContent.trim();
    if (content.length > 10) {
        tsl[i].textContent = new Intl.DateTimeFormat('en-IN',
            { dateStyle: 'long', timeStyle: 'long', hourCycle: 'h24' })
            .format(new Date(content));
    }
}