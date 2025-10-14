#!/usr/bin/env python3
"""
Scan the repository for LangManager.GetString("<key>", "<default>") usages,
compare keys to res/langs/en-US.json and res/langs/vi-VN.json, and add missing
keys to both files using the default as the value.

Options:
  --noadd    Only print missing keys; don't modify language files.
  --sort     After adding, sort keys alphabetically in the JSON files.

Usage examples:
  python tools/findMissingLangKeys.py         # add missing keys
  python tools/findMissingLangKeys.py --noadd
  python tools/findMissingLangKeys.py --sort
"""
import argparse
import json
import re
from pathlib import Path
import sys


def find_usages(root: Path):
    pattern = re.compile(r"LangManager\.GetString\(\s*([\"'])(?P<key>.+?)\1\s*,\s*([\"'])(?P<def>.*?)\3\s*\)")
    usages = {}
    for p in root.rglob('*.cs'):
        try:
            text = p.read_text(encoding='utf-8')
        except Exception:
            continue
        for m in pattern.finditer(text):
            key = m.group('key')
            default = m.group('def')
            # Keep the last default seen for duplicated keys (later occurrences override earlier ones)
            usages[key] = default
    return usages


def load_json(path: Path):
    if not path.exists():
        return {}
    try:
        return json.loads(path.read_text(encoding='utf-8'))
    except Exception as e:
        print(f'Failed to read {path}: {e}', file=sys.stderr)
        return {}


def write_json(path: Path, data: dict, sort_keys: bool = False):
    try:
        path.write_text(json.dumps(data, ensure_ascii=False, indent=4, sort_keys=sort_keys) + "\n", encoding='utf-8')
    except Exception as e:
        print(f'Failed to write {path}: {e}', file=sys.stderr)


def main():
    parser = argparse.ArgumentParser(description='Find and add missing language keys')
    parser.add_argument('--noadd', action='store_true', help="Don't modify language files, only print missing keys")
    parser.add_argument('--sort', action='store_true', help='Sort language files after adding')
    parser.add_argument('--overwrite', action='store_true', help='Overwrite existing keys in language files with latest defaults')
    parser.add_argument('--root', type=Path, default=Path('.'), help='Repository root (default: .)')
    parser.add_argument('--langs', type=Path, default=Path('res/langs'), help='Languages folder')
    args = parser.parse_args()

    root = args.root
    langs_dir = args.langs
    en = langs_dir / 'en-US.json'
    vi = langs_dir / 'vi-VN.json'

    usages = find_usages(root)
    if not usages:
        print('No LangManager.GetString usages found.')
        return

    en_data = load_json(en)
    vi_data = load_json(vi)

    # Determine missing keys (present in usages but not present in both language files)
    missing = {}
    for k, default in usages.items():
        in_en = k in en_data
        in_vi = k in vi_data
        if not (in_en and in_vi):
            missing[k] = default

    if not missing:
        print('No missing keys. All keys present in both language files.')
        return

    # Print missing keys
    print('Missing keys:')
    for k, default in sorted(missing.items()):
        print(f'- {k}: "{default}"')

    if args.noadd:
        return

    # Add missing keys to both JSONs
    for k, default in missing.items():
        if k not in en_data:
            en_data[k] = default
        if k not in vi_data:
            vi_data[k] = default

    # If overwrite flag is set, overwrite existing keys in language files
    if args.overwrite:
        for k, default in usages.items():
            en_data[k] = default
            vi_data[k] = default

    # Write back
    write_json(en, en_data, sort_keys=args.sort)
    write_json(vi, vi_data, sort_keys=args.sort)

    print(f'Added {len(missing)} keys to {en} and {vi}.')
    if args.sort:
        print('Files were written with keys sorted (sort_keys=True).')


if __name__ == '__main__':
    main()
