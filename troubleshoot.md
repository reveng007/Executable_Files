The error means VirtualBox Guest Additions kernel modules aren't loaded in your Kali VM. Here's how to fix it:

## Fix VBoxClient / Clipboard Sharing

**1. Reinstall/rebuild Guest Additions inside Kali VM:**
```bash
sudo apt update
sudo apt install -y virtualbox-guest-x11 virtualbox-guest-utils virtualbox-guest-dkms
```

**2. If that doesn't work, rebuild against current kernel:**
```bash
sudo apt install -y dkms linux-headers-$(uname -r)
sudo rcvboxadd setup
```

**3. Restart the VBoxClient services:**
```bash
sudo killall VBoxClient 2>/dev/null
VBoxClient --clipboard
VBoxClient --draganddrop
```

Or restart all at once:
```bash
VBoxClient-all
```

**4. Reboot the VM** — kernel module changes usually need a full reboot:
```bash
sudo reboot
```

---

## Verify Guest Additions are loaded after reboot:
```bash
lsmod | grep vbox
# Should show: vboxguest, vboxsf, vboxvideo
```

---

## VirtualBox Host Side — Double check:
- **Devices → Shared Clipboard → Bidirectional**
- **Devices → Drag and Drop → Bidirectional**

---

## If kernel module still fails (common after Kali updates):
```bash
sudo apt install -y --reinstall virtualbox-guest-dkms
sudo dkms autoinstall
```

The root cause is usually a kernel update outpacing the Guest Additions DKMS rebuild. The `dkms autoinstall` command forces a recompile against the running kernel.
