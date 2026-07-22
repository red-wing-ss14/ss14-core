#!/usr/bin/env python3
# SPDX-FileCopyrightText: 2024 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
# SPDX-FileCopyrightText: 2024 Piras314 <p1r4s@proton.me>
# SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
# SPDX-FileCopyrightText: 2025 Aiden <aiden@djkraz.com>
#
# SPDX-License-Identifier: AGPL-3.0-or-later

import requests
import os
import subprocess
from typing import Iterable

PUBLISH_TOKEN = os.environ["PUBLISH_TOKEN"]
VERSION = os.environ["GITHUB_SHA"]

RELEASE_DIR = "release"

#
# CONFIGURATION PARAMETERS
# Forks should change these to publish to their own infrastructure.
#
ROBUST_CDN_URL = os.environ.get("ROBUST_CDN_URL", "https://main-cdn.reserve-station.space/") # reserve
FORK_ID = os.environ.get("FORK_ID", "reserve") # reserve

def main():
    import argparse
    parser = argparse.ArgumentParser(description="Publish build to Robust.Cdn")
    parser.add_argument("--fork-id", default=FORK_ID, help="Fork ID on Robust.Cdn")
    parser.add_argument("--cdn-url", default=ROBUST_CDN_URL, help="Robust.Cdn base URL")
    args, _ = parser.parse_known_args()

    fork_id = args.fork_id
    cdn_url = args.cdn_url
    if not cdn_url.endswith("/"):
        cdn_url += "/"

    session = requests.Session()
    session.headers = {
        "Authorization": f"Bearer {PUBLISH_TOKEN}",
    }

    print(f"Starting publish on Robust.Cdn for version {VERSION} (fork: {fork_id})")

    data = {
        "version": VERSION,
        "engineVersion": get_engine_version(),
    }
    headers = {
        "Content-Type": "application/json"
    }
    resp = session.post(f"{cdn_url}fork/{fork_id}/publish/start", json=data, headers=headers)
    if not resp.ok:
        print(f"Failed to start publish: HTTP {resp.status_code} {resp.reason}\nServer response: {resp.text}")
    resp.raise_for_status()
    print("Publish successfully started, adding files...")

    for file in get_files_to_publish():
        print(f"Publishing {file}")
        with open(file, "rb") as f:
            headers = {
                "Content-Type": "application/octet-stream",
                "Robust-Cdn-Publish-File": os.path.basename(file),
                "Robust-Cdn-Publish-Version": VERSION
            }
            resp = session.post(f"{cdn_url}fork/{fork_id}/publish/file", data=f, headers=headers)

        if not resp.ok:
            print(f"Failed to publish file {file}: HTTP {resp.status_code} {resp.reason}\nServer response: {resp.text}")
        resp.raise_for_status()

    print("Successfully pushed files, finishing publish...")

    data = {
        "version": VERSION
    }
    headers = {
        "Content-Type": "application/json"
    }
    resp = session.post(f"{cdn_url}fork/{fork_id}/publish/finish", json=data, headers=headers)
    if not resp.ok:
        print(f"Failed to finish publish: HTTP {resp.status_code} {resp.reason}\nServer response: {resp.text}")
    resp.raise_for_status()

    print("SUCCESS!")


def get_files_to_publish() -> Iterable[str]:
    for file in os.listdir(RELEASE_DIR):
        yield os.path.join(RELEASE_DIR, file)


def get_engine_version() -> str:
    import xml.etree.ElementTree as ET
    tree = ET.parse(os.path.join("RobustToolbox", "MSBuild", "Robust.Engine.Version.props"))
    version = tree.getroot().find(".//Version").text.strip()
    return version
    # proc = subprocess.run(["git", "describe","--tags", "--abbrev=0"], stdout=subprocess.PIPE, cwd="RobustToolbox", check=True, encoding="UTF-8")
    # tag = proc.stdout.strip()
    # assert tag.startswith("v")
    # return tag[1:] # Cut off v prefix.

if __name__ == '__main__':
    main()
