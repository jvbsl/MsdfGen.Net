#!/bin/bash

REPO="jvbsl/msdfgen"

LATEST_RELEASE_ASSETS=( $(curl -s https://api.github.com/repos/$REPO/releases | jq ".[0].assets | .[] | select(.name | contains(\"openmp\")) | .browser_download_url") )

get_destination() {
    destination=$(echo "$1" | grep -oP "\-\K[^\-]*\-[^\-]*$")
    echo "$destination"
}

mkdir tmp 2> /dev/null
pushd tmp > /dev/null

mkdir runtimes 2> /dev/null
for i in "${LATEST_RELEASE_ASSETS[@]}"
do
    temp="${i%\"}"
    temp="${temp#\"}"
    wget  -q --show-progress -c -N "$temp"
    
    filename=$(basename $temp)
    if [ ${filename: -4} == ".zip" ]
    then
        name=${filename: 0:-4}
        destination="runtimes/$(get_destination $name)/native"
        mkdir -p "$destination" 2> /dev/null
        pushd "$destination" > /dev/null
        unzip -u "../../../$filename" '*_shared*' -x '*.lib' 2&> /dev/null
        popd > /dev/null
    elif [ ${filename: -7} == ".tar.gz" ]
    then
        name=${filename: 0:-7}
        destination="runtimes/$(get_destination $name)/native"
        mkdir -p "$destination" 2> /dev/null
        pushd "$destination" > /dev/null
        tar --overwrite -xvzf "../../../$filename" --wildcards "*_shared*" 2&> /dev/null
        popd > /dev/null
    fi
    
    echo "Runtime: $destination"
done

popd > /dev/null