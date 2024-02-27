'use server'

import { Auction, PagedResult } from "@/types";

export async function getData(query: string): Promise<PagedResult<Auction>> {
    const res = await fetch(`http://localhost:5003/search${query}`)
    if (!res.ok) {
        throw new Error('Failed to fetch data')
    }

    return res.json();
}