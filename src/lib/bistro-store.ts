// Simple client-side data store using localStorage.
// Replace these functions with real API calls when backend is ready.

export type Categoria = "Almoços" | "Hambúrgueres" | "Bebidas" | "Doces";

export interface Produto {
  id: string;
  nome: string;
  preco: number;
  categoria: Categoria;
}

export interface ItemVenda {
  produtoId: string;
  nome: string;
  preco: number;
  quantidade: number;
}

export interface Venda {
  id: string;
  data: string; // ISO
  itens: ItemVenda[];
  total: number;
}

const PRODUTOS_KEY = "bc_bistro_produtos";
const VENDAS_KEY = "bc_bistro_vendas";

const uid = () => Math.random().toString(36).slice(2, 10);

const produtosIniciais: Produto[] = [
  { id: uid(), nome: "Almoço — Voluntário", preco: 35, categoria: "Almoços" },
  { id: uid(), nome: "Almoço", preco: 45, categoria: "Almoços" },
  { id: uid(), nome: "Hambúrguer com batata", preco: 35, categoria: "Hambúrgueres" },
  { id: uid(), nome: "Hambúrguer sem batata", preco: 30, categoria: "Hambúrgueres" },
  { id: uid(), nome: "Hambúrguer com batata (Voluntário)", preco: 25, categoria: "Hambúrgueres" },
  { id: uid(), nome: "Hambúrguer sem batata (Voluntário)", preco: 20, categoria: "Hambúrgueres" },
  { id: uid(), nome: "Água", preco: 4, categoria: "Bebidas" },
  { id: uid(), nome: "Coca-Cola normal", preco: 7, categoria: "Bebidas" },
  { id: uid(), nome: "Coca-Cola zero", preco: 7, categoria: "Bebidas" },
  { id: uid(), nome: "Guaraná", preco: 6, categoria: "Bebidas" },
  { id: uid(), nome: "Guaraná zero", preco: 6, categoria: "Bebidas" },
  { id: uid(), nome: "Energético", preco: 12, categoria: "Bebidas" },
  { id: uid(), nome: "Brownie", preco: 8, categoria: "Doces" },
  { id: uid(), nome: "Alfajor", preco: 7, categoria: "Doces" },
  { id: uid(), nome: "Chiclete", preco: 2, categoria: "Doces" },
];

function isBrowser() {
  return typeof window !== "undefined";
}

export function getProdutos(): Produto[] {
  if (!isBrowser()) return produtosIniciais;
  const raw = localStorage.getItem(PRODUTOS_KEY);
  if (!raw) {
    localStorage.setItem(PRODUTOS_KEY, JSON.stringify(produtosIniciais));
    return produtosIniciais;
  }
  try {
    return JSON.parse(raw);
  } catch {
    return produtosIniciais;
  }
}

export function saveProdutos(p: Produto[]) {
  localStorage.setItem(PRODUTOS_KEY, JSON.stringify(p));
}

export function addProduto(p: Omit<Produto, "id">): Produto {
  const novo = { ...p, id: uid() };
  const all = [...getProdutos(), novo];
  saveProdutos(all);
  return novo;
}

export function updateProduto(id: string, patch: Partial<Omit<Produto, "id">>) {
  const all = getProdutos().map((p) => (p.id === id ? { ...p, ...patch } : p));
  saveProdutos(all);
}

export function deleteProduto(id: string) {
  saveProdutos(getProdutos().filter((p) => p.id !== id));
}

export function getVendas(): Venda[] {
  if (!isBrowser()) return [];
  const raw = localStorage.getItem(VENDAS_KEY);
  if (!raw) return [];
  try {
    return JSON.parse(raw);
  } catch {
    return [];
  }
}

export function addVenda(itens: ItemVenda[]): Venda {
  const total = itens.reduce((s, i) => s + i.preco * i.quantidade, 0);
  const venda: Venda = { id: uid(), data: new Date().toISOString(), itens, total };
  const all = [venda, ...getVendas()];
  localStorage.setItem(VENDAS_KEY, JSON.stringify(all));
  return venda;
}

export const formatBRL = (v: number) =>
  v.toLocaleString("pt-BR", { style: "currency", currency: "BRL" });
